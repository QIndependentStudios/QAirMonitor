using System;

namespace QAirMonitor.Domain.Sensors
{
    public class TempHumidityReadingResult
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime ReadingDateTime { get; set; }
        public int Attempts { get; set; }
    }
}
