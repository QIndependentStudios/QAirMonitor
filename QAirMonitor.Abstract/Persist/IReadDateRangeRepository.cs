using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QAirMonitor.Abstract.Persist
{
    public interface IReadDateRangeRepository<T>
    {
        Task<IEnumerable<T>> GetInDateRangeAsync(DateTime startDate, DateTime? endDate = null);
    }
}
