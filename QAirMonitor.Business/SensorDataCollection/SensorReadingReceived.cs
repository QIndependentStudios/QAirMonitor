using QAirMonitor.Abstract.Business;
using QAirMonitor.Domain.Models;
using System;

namespace QAirMonitor.Business.SensorDataCollection
{
    public class SensorReadingReceived : EventArgs, ISensorReadingReceivedEventArgs<ReadingModel>
    {
        public SensorReadingReceived(ReadingModel reading, int attempts)
        {
            NewReading = reading;
            Attempts = attempts;
        }

        public ReadingModel NewReading { get; private set; }

        public int Attempts { get; private set; }
    }
}
