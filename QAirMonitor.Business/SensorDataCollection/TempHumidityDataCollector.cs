using QAirMonitor.Abstract.Business;
using QAirMonitor.Abstract.Persist;
using QAirMonitor.Abstract.Sensors;
using QAirMonitor.Business.Logging;
using QAirMonitor.Domain.Models;
using QAirMonitor.Domain.Sensors;
using QAirMonitor.Persist.Repositories;
using System;

namespace QAirMonitor.Business.SensorDataCollection
{
    public class TempHumidityDataCollector : ISensorDataCollector<ReadingModel>
    {
        private readonly TimeSpan DataCollectionInterval = TimeSpan.FromMinutes(5);
        private readonly IIntervalTimer _timer;
        private readonly IWriteRepository<ReadingModel> _writeRepo;
        private readonly ITempHumiditySensor<TempHumidityReadingResult> _sensor;

        public TempHumidityDataCollector(IIntervalTimer timer, ITempHumiditySensor<TempHumidityReadingResult> sensor)
        {
            _timer = timer;
            _timer.Tick += Timer_Tick;

            _writeRepo = new HistoricalReadingRepository();
            _sensor = sensor;
        }

        public async void Start()
        {
            await Logger.LogAsync($"{nameof(TempHumidityDataCollector)}", "Data collection started.");
            _timer.Start();
        }

        public async void Stop()
        {
            await Logger.LogAsync($"{nameof(TempHumidityDataCollector)}", "Data collection stopped.");
            _timer.Stop();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            var readingResult = await _sensor.GetReadingAsync();

            if (readingResult == null)
            {
                OnReadingReceived(null, readingResult.Attempts);
                return;
            }

            var reading = new ReadingModel
            {
                Temperature = readingResult.Temperature,
                Humidity = readingResult.Humidity,
                ReadingDateTime = readingResult.ReadingDateTime
            };
            await _writeRepo.WriteAsync(reading);
            OnReadingReceived(reading, readingResult.Attempts);
        }

        protected void OnReadingReceived(ReadingModel reading, int attempts)
        {
            ReadingReceived?.Invoke(this, new SensorReadingReceivedEventArgs(reading, attempts));
        }

        public event EventHandler<ISensorReadingReceivedEventArgs<ReadingModel>> ReadingReceived;
    }
}
