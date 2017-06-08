using System;

namespace QAirMonitor.Domain.Models
{
    public class ReadingModel
    {
        public int ReadingID { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime ReadingDateTime { get; set; }
    }
}
