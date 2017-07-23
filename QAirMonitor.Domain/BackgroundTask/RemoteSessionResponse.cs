using QAirMonitor.Domain.Models;
using System.Collections;
using System.Collections.Generic;

namespace QAirMonitor.Domain.BackgroundTask
{
    public class RemoteSessionResponse
    {
        public RemoteSessionResponse(IList readings)
        {
            Readings = readings;
        }

        public IList Readings { get; } = new List<ReadingModel>();
    }
}
