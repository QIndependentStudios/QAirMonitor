using System;

namespace QAirMonitor.Domain.BackgroundTask
{
    public class RemoteSessionRequest
    {
        public RemoteSessionRequest(DateTime startDateTime)
        {
            StartDateTime = startDateTime;
        }

        public DateTime StartDateTime { get; }
    }
}
