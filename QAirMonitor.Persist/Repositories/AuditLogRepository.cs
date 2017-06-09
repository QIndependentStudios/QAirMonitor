using Microsoft.EntityFrameworkCore;
using QAirMonitor.Abstract.Persist;
using QAirMonitor.Domain.Enums;
using QAirMonitor.Domain.Models;
using QAirMonitor.Persist.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QAirMonitor.Persist.Repositories
{
    public class AuditLogRepository : IAuditLogRepository<AuditLogModel, AuditLogEventType>, IWriteRepository<AuditLogModel>
    {
        public async Task LogAsync(string message, AuditLogEventType eventType, DateTime eventDateTime)
        {
            var log = new AuditLogModel
            {
                Message = message,
                EventType = eventType,
                EventDateTime = eventDateTime
            };

            await WriteAsync(log);
        }

        public async Task<IEnumerable<AuditLogModel>> GetLogsAsync()
        {
            using (var context = new AppDataContext())
            {
                return await context.AuditLogEntries
                    .OrderByDescending(l => l.EventDateTime)
                    .ThenByDescending(l => l.AuditLogID)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<AuditLogModel>> GetLogsAsync(AuditLogEventType eventType)
        {
            using (var context = new AppDataContext())
            {
                return await context.AuditLogEntries
                    .Where(l => l.EventType == eventType)
                    .OrderByDescending(l => l.EventDateTime)
                    .ThenByDescending(l => l.AuditLogID)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<AuditLogModel>> GetLogsAsync(AuditLogEventType eventType, DateTime startDate, DateTime endDate)
        {
            using (var context = new AppDataContext())
            {
                return await context.AuditLogEntries
                    .Where(l => l.EventType == eventType)
                    .Where(l => l.EventDateTime >= startDate)
                    .Where(l => l.EventDateTime <= endDate)
                    .OrderByDescending(l => l.EventDateTime)
                    .ThenByDescending(l => l.AuditLogID)
                    .ToListAsync();
            }
        }

        public async Task WriteAsync(AuditLogModel model)
        {
            using (var context = new AppDataContext())
            {
                await context.AuditLogEntries.AddAsync(model);
                await context.SaveChangesAsync();
            }
        }
    }
}
