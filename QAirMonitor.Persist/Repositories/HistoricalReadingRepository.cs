using Microsoft.EntityFrameworkCore;
using QAirMonitor.Abstract.Persist;
using QAirMonitor.Domain.Models;
using QAirMonitor.Persist.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace QAirMonitor.Persist.Repositories
{
    public class HistoricalReadingRepository : IReadAllRepository<ReadingModel>,
        IReadDateRangeRepository<ReadingModel>,
        IWriteRepository<ReadingModel>
    {
        public async Task<IEnumerable<ReadingModel>> GetAllAsync()
        {
            using (var context = new AppDataContext())
            {
                return await context.Readings
                    .OrderByDescending(r => r.ReadingDateTime)
                    .ThenByDescending(r => r.ReadingID)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<ReadingModel>> GetInDateRangeAsync(DateTime startDate,
            DateTime? endDate = null)
        {
            using (var context = new AppDataContext())
            {
                var query = context.Readings
                    .Where(r => r.ReadingID != 1 && r.ReadingDateTime > startDate);

                if (endDate != null)
                    query = query.Where(r => r.ReadingDateTime <= endDate);

                return await query.OrderByDescending(r => r.ReadingDateTime)
                    .ThenByDescending(r => r.ReadingID)
                    .ToListAsync();
            }
        }

        public async Task WriteAsync(ReadingModel model)
        {
            using (var context = new AppDataContext())
            {
                await context.Readings.AddAsync(model);
                await context.SaveChangesAsync();
            }
        }
    }
}
