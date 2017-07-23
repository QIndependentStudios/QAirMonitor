using QAirMonitor.Abstract.Persist;
using QAirMonitor.Domain.Models;
using QAirMonitor.Persist.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QAirMonitor.Business.BackgroundTask
{
    public class RemoteSessionAppServiceWorker
    {
        private readonly IReadDateRangeRepository<ReadingModel> _repo;

        public RemoteSessionAppServiceWorker()
        {
            _repo = new HistoricalReadingRepository();
        }

        public async Task<IEnumerable<ReadingModel>> GetReadingsAsync(DateTime startDateTime)
        {
            return await _repo.GetInDateRangeAsync(startDateTime);
        }
    }
}
