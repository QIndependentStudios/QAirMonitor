using QAirMonitor.Abstract.Persist;
using QAirMonitor.Domain.Enums;
using QAirMonitor.Domain.Models;
using QAirMonitor.Persist.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QAirMonitor.Business.Logging
{
    public class Logger
    {
        private static readonly IAuditLogRepository<AuditLogModel, AuditLogEventType> _repo = new AuditLogRepository();

        private static string CreateLogMessage(string caller, string message, int maxLength = 2000)
        {
            var str = $"{caller}: {message}";
            return str == null
                ? string.Empty
                : str.Substring(0, Math.Min(maxLength, str.Length));
        }

        public static async Task LogAsync(string caller, string message, AuditLogEventType eventType = AuditLogEventType.Event)
        {
            await _repo.LogAsync(CreateLogMessage(caller, message), eventType, DateTime.Now);
        }

        public static async Task LogExceptionAsync(string caller, Exception e)
        {
            await LogAsync(caller, $"Exception thrown. {e.Message} {e.StackTrace}", AuditLogEventType.Error);
        }

        public static async Task<IEnumerable<AuditLogModel>> GetLogsAsync()
        {
            return await _repo.GetLogsAsync();
        }

        public static async Task<IEnumerable<AuditLogModel>> GetLogsAsync(AuditLogEventType eventType)
        {
            return await _repo.GetLogsAsync(eventType);
        }

        public static async Task<IEnumerable<AuditLogModel>> GetLogsAsync(AuditLogEventType eventType, DateTime startDate, DateTime endDate)
        {
            return await _repo.GetLogsAsync(eventType, startDate, endDate);
        }
    }
}
