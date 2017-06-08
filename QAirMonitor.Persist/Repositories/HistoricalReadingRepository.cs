using Microsoft.EntityFrameworkCore;
using QAirMonitor.Abstract.Persist;
using QAirMonitor.Domain.Models;
using QAirMonitor.Persist.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QAirMonitor.Persist.Repositories
{
    public class HistoricalReadingRepository : IReadAllRepository<ReadingModel>, IWriteRepository<ReadingModel>
    {
        public async Task<IEnumerable<ReadingModel>> GetAllAsync()
        {
            using (var context = new AppDataContext())
            {
                return await context.Readings.ToListAsync();
            }
        }

        public async Task WriteAsync(ReadingModel model)
        {
            using (var context = new AppDataContext())
            {
                await context.Readings.AddAsync(model);
            }
        }
    }
}
