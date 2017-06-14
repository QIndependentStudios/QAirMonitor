using QAirMonitor.Abstract.Sensors;
using QAirMonitor.Business.Logging;
using QAirMonitor.Domain.Sensors;
using System;
using System.Threading.Tasks;

namespace QAirMonitor.Hardware.UWP.Sensors
{
    public class VirtualTempHumiditySensor : ITempHumiditySensor<TempHumidityReadingResult>
    {
        private double _minTempLimit;
        private double _maxTempLimit;
        private double _minHumidityLimit;
        private double _maxHumidityLimit;

        private readonly Random _rand = new Random();

        public VirtualTempHumiditySensor()
            : this(2, 5, 65, 75)
        { }

        public VirtualTempHumiditySensor(double minTempLimit,
            double maxTempLimit,
            double minHumidityLimit,
            double maxHumidityLimit)
        {
            _minTempLimit = minTempLimit;
            _maxTempLimit = maxTempLimit;
            _minHumidityLimit = minHumidityLimit;
            _maxHumidityLimit = maxHumidityLimit;
        }

        public async Task<TempHumidityReadingResult> GetReadingAsync()
        {
            var reading = new TempHumidityReadingResult
            {
                Temperature = _rand.Next((int)(_minTempLimit * 100), (int)(_maxTempLimit * 100)) / 100.0,
                Humidity = _rand.Next((int)(_minHumidityLimit * 100), (int)(_maxHumidityLimit * 100)) / 100.0,
                ReadingDateTime = DateTime.Now,
                Attempts = 1,
                IsSuccessful = true
            };

            await Logger.LogAsync($"{nameof(VirtualTempHumiditySensor)}", $"Successful reading. {reading.Temperature:0.0}°C, {reading.Humidity:0.0}%, {reading.Attempts} attempt(s).");

            return reading;
        }
    }
}
