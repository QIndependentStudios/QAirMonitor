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

        public async Task RunAsync()
        {
            await Logger.LogAsync(nameof(TempHumidityNotificationWorker), "Worker started.");

            var readings = await _repo.GetInDateRangeAsync(DateTime.Now.AddHours(-1 * Scope), DateTime.Now);

            var data = new TempHumidityNotificationData(readings);

            var summary = $"QAirMonitor reports that in the past {Scope} hour(s), temperatures averaged {data.AverageTemperature:0.00}°C and humidity averaged {data.AverageHumidity:0.00}%.";
            var readout = string.Empty;

            foreach (var reading in data.Readings)
            {
                readout += $"{reading.ReadingDateTime:M/d/yy h:mm:ss tt}\t{reading.Temperature:0.00}°C\t{reading.Humidity:0.00}%\n";
            }

            var emailNotifier = new EmailNotifier();
            await emailNotifier.SendNotificationAsync($"{summary}\n\n\n{readout}");


            var iftttPostData = new Dictionary<string, string>
            {
                { "value1", summary },
                { "value2", $"\n\n{readout}" }
            };

            var iftttNotifier = new IFTTTNotifier();
            await iftttNotifier.SendNotificationAsync(iftttPostData);

            await Logger.LogAsync(nameof(TempHumidityNotificationWorker), "Worker completed.");
        }
    }
}
