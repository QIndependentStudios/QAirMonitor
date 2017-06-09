using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QAirMonitor.Abstract.Persist
{
    public interface IAuditLogRepository<T, TEventType>
    {
        Task LogAsync(string message, TEventType eventType, DateTime eventDateTime);
        Task<IEnumerable<T>> GetLogsAsync();
        Task<IEnumerable<T>> GetLogsAsync(TEventType eventType);
        Task<IEnumerable<T>> GetLogsAsync(TEventType eventType, DateTime startDate, DateTime endDate);
    }
}
