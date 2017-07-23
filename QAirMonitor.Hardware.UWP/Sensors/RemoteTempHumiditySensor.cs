using QAirMonitor.Abstract.Sensors;
using QAirMonitor.Domain.Sensors;
using System;
using System.Threading.Tasks;

namespace QAirMonitor.Hardware.UWP.Sensors
{
    public class RemoteTempHumiditySensor : ITempHumiditySensor<TempHumidityReadingResult>
    {
        public Task<TempHumidityReadingResult> GetReadingAsync()
        {
            throw new NotImplementedException();
        }
    }
}
