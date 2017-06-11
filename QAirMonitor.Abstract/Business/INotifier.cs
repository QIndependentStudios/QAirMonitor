using System.Threading.Tasks;

namespace QAirMonitor.Abstract.Business
{
    public interface INotifier<TData>
    {
        Task SendNotificationAsync (TData data);
    }
}
