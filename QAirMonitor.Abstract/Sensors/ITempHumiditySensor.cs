using System.Threading.Tasks;

namespace QAirMonitor.Abstract.Sensors
{
    public interface ITempHumiditySensor<TReading>
    {
        Task<TReading> GetReadingAsync();
    }
}
