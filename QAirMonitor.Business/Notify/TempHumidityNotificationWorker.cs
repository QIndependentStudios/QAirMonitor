using QAirMonitor.Abstract.Persist;
using QAirMonitor.Business.Logging;
using QAirMonitor.Domain.Models;
using QAirMonitor.Domain.Notify;
using QAirMonitor.Persist.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QAirMonitor.Business.Notify
{
    public class TempHumidityNotificationWorker
    {
        private const int Scope = 2;
        private readonly IReadDateRangeRepository<ReadingModel> _repo;

        public TempHumidityNotificationWorker()
        {
            _repo = new HistoricalReadingRepository();
        }

        public async Task RunAsync(NotificationSettings settings)
        {
            await Logger.LogAsync(nameof(TempHumidityNotificationWorker), "Worker started.");

            if (settings.NotificationStartTime != settings.NotificationEndTime &&
                (DateTime.Now.TimeOfDay < settings.NotificationStartTime ||
                DateTime.Now.TimeOfDay > settings.NotificationEndTime))
            {
                await Logger.LogAsync(nameof(TempHumidityNotificationWorker), "Worker completed: No action outside of notification active times.");
                return;
            }

            var readings = await _repo.GetInDateRangeAsync(DateTime.Now.AddHours(-1 * Scope), DateTime.Now);

            var data = new TempHumidityNotificationData(readings);

            var summary = $"QAirMonitor reports that in the past {Scope} hour(s), temperatures averaged {data.AverageTemperature:0.00}°C and humidity averaged {data.AverageHumidity:0.00}%.";
            var readout = string.Empty;

            foreach (var reading in data.Readings)
            {
                readout += $"{reading.ReadingDateTime:M/d/yy h:mm:ss tt}\t{reading.Temperature:0.00}°C\t{reading.Humidity:0.00}%\n";
            }

            if (settings.IsEmailNotificationEnabled)
            {
                try
                {
                    var emailNotifier = new EmailNotifier();
                    await emailNotifier.SendNotificationAsync($"{summary}\n\n\n{readout}", settings);
                }
                catch (Exception e)
                {
                    await Logger.LogExceptionAsync(nameof(TempHumidityNotificationWorker), e);
                }
            }
            
            if (settings.IsIftttNotificationEnabled)
            {
                var iftttPostData = new Dictionary<string, string>
                {
                    { "value1", summary },
                    { "value2", $"\n\n{readout}" }
                };

                try
                {
                    var iftttNotifier = new IFTTTNotifier();
                    await iftttNotifier.SendNotificationAsync(iftttPostData, settings);
                }
                catch (Exception e)
                {
                    await Logger.LogExceptionAsync(nameof(TempHumidityNotificationWorker), e);
                }
            }

            await Logger.LogAsync(nameof(TempHumidityNotificationWorker), "Worker completed.");
        }
    }
}
