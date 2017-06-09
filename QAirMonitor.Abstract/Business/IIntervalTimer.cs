using System;

namespace QAirMonitor.Abstract.Business
{
    public interface IIntervalTimer
    {
        void Start();
        void Stop();
        event EventHandler Tick;
    }
}
