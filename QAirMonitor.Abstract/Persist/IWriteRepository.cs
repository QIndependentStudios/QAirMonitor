using System.Threading.Tasks;

namespace QAirMonitor.Abstract.Persist
{
    public interface IWriteRepository<T>
    {
        Task WriteAsync(T model);
    }
}
