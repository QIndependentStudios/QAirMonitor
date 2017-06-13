using System.Threading.Tasks;

namespace QAirMonitor.Abstract.Business
{
    public interface INotifier<TData, TSettings>
    {
        Task SendNotificationAsync(TData data, TSettings settings);
    }
}
