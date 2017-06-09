using QAirMonitor.Abstract.Business;
using QAirMonitor.Domain.Models;
using System;

namespace QAirMonitor.Business.SensorDataCollection
{
    public class SensorReadingReceivedEventArgs : EventArgs, ISensorReadingReceivedEventArgs<ReadingModel>
    {
        public SensorReadingReceivedEventArgs(ReadingModel reading, int attempts)
        {
            NewReading = reading;
            Attempts = attempts;
        }

        public ReadingModel NewReading { get; private set; }

        public int Attempts { get; private set; }
    }
}
