using System.Collections.Generic;
using System.Threading.Tasks;

namespace QAirMonitor.Abstract.Persist
{
    public interface IReadAllRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
    }
}
