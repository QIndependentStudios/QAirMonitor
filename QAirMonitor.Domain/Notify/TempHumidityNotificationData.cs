using QAirMonitor.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace QAirMonitor.Domain.Notify
{
    public class TempHumidityNotificationData
    {
        public TempHumidityNotificationData(IEnumerable<ReadingModel> readings)
        {
            Readings = readings;
        }

        public IEnumerable<ReadingModel> Readings { get; protected set; }

        public double AverageTemperature => Readings.Select(r => r.Temperature)
            .DefaultIfEmpty(0.0)
            .Aggregate((x, y) => x + y) / Readings.Count();

        public double AverageHumidity => Readings.Select(r => r.Humidity)
            .DefaultIfEmpty(0.0)
            .Aggregate((x, y) => x + y) / Readings.Count();
    }
}
