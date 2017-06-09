using System;

namespace QAirMonitor.Abstract.Business
{
    public interface ISensorDataCollector<TReading>
    {
        TReading LastReading { get; }
        int LastReadingAttempts { get; }
        void Start();
        void Stop();
        event EventHandler<ISensorReadingReceivedEventArgs<TReading>> ReadingReceived;
    }
}
