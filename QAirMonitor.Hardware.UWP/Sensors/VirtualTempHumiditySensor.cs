using QAirMonitor.Abstract.Sensors;
using QAirMonitor.Domain.Sensors;
using System;
using System.Threading.Tasks;

namespace QAirMonitor.Hardware.UWP.Sensors
{
    public class VirtualTempHumiditySensor : ITempHumiditySensor<TempHumidityReadingResult>
    {
        private double _minTempLimit = 2;
        private double _maxTempLimit = 5;
        private double _minHumidityLimit = 40;
        private double _maxHumidityLimit = 65;

        private readonly Random _rand = new Random();

        public VirtualTempHumiditySensor()
            : this(2, 5, 40, 65)
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

        public Task<TempHumidityReadingResult> GetReadingAsync()
        {
            return Task.FromResult(new TempHumidityReadingResult
            {
                Temperature = _rand.Next((int)(_minTempLimit * 100), (int)(_maxTempLimit * 100)) / 100.0,
                Humidity = _rand.Next((int)(_minHumidityLimit * 100), (int)(_maxHumidityLimit * 100)) / 100.0,
                ReadingDateTime = DateTime.Now,
                Attempts = 1
            });
        }
    }
}
