using QAirMonitor.Abstract.Business;
using QAirMonitor.UWP.Utils;
using System;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace QAirMonitor.UWP.Timers
{
    public class SnappedIntervalTimer : IIntervalTimer
    {
        private readonly TimeSpan Interval;
        private ThreadPoolTimer _timer;

        #region Constructors
        public SnappedIntervalTimer()
            : this(TimeSpan.FromMinutes(5))
        { }

        public SnappedIntervalTimer(TimeSpan interval)
        {
            Interval = interval;
        }
        #endregion

        private void TimeElapsedHandler(ThreadPoolTimer timer = null)
        {
            OnTick();
        }

        public async void Start()
        {
            var preferredStartTime = DateTime.Now.Ceiling(Interval);
            var delay = preferredStartTime - DateTime.Now;

            await Task.Delay((int)delay.TotalMilliseconds);

            OnTick();

            _timer = ThreadPoolTimer.CreatePeriodicTimer(TimeElapsedHandler,
                Interval);
        }

        public void Stop()
        {
            if (_timer == null)
                return;

            _timer.Cancel();
            _timer = null;
        }

        public void OnTick()
        {
            Tick?.Invoke(this, new EventArgs());
        }

        public event EventHandler Tick;
    }
}
