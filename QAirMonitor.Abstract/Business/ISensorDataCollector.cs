using System;

namespace QAirMonitor.Abstract.Business
{
    public interface ISensorDataCollector<TReading>
    {
        void Start();
        void Stop();
        event EventHandler<ISensorReadingReceivedEventArgs<TReading>> ReadingReceived;
    }
}
