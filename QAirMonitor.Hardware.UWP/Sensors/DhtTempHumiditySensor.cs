using QAirMonitor.Abstract.Sensors;
using QAirMonitor.Business.Logging;
using QAirMonitor.Domain.Enums;
using QAirMonitor.Domain.Sensors;
using Sensors.Dht;
using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace QAirMonitor.Hardware.UWP.Sensors
{
    public class DhtTempHumiditySensor : ITempHumiditySensor<TempHumidityReadingResult>
    {
        private GpioPin _sensorDataPin;
        private IDht _dhtSensor;

        public DhtTempHumiditySensor()
        {
            _sensorDataPin = GpioController.GetDefault().OpenPin(4, GpioSharingMode.Exclusive);
            _dhtSensor = new Dht22(_sensorDataPin, GpioPinDriveMode.Input);
        }

        public async Task<TempHumidityReadingResult> GetReadingAsync()
        {
            var reading = new TempHumidityReadingResult();
            try
            {
                DhtReading hdtReading = new DhtReading();

                hdtReading = await _dhtSensor.GetReadingAsync(250).AsTask();

                if (hdtReading.IsValid)
                {
                    reading.Temperature = hdtReading.Temperature;
                    reading.Humidity = hdtReading.Humidity;
                    reading.ReadingDateTime = DateTime.Now;
                    reading.Attempts = hdtReading.RetryCount + 1;

                    await Logger.LogAsync($"{nameof(DhtTempHumiditySensor)}", $"Successful reading. {reading.Temperature}°C, {reading.Humidity}%, {reading.Attempts} attempt(s).");

                    return reading;
                }
                else
                {
                    await Logger.LogAsync($"{nameof(DhtTempHumiditySensor)}", $"Failed reading. {reading.Attempts} attempt(s).", AuditLogEventType.Warning);
                    return null;
                }
            }
            catch (Exception e)
            {
                await Logger.LogExceptionAsync(nameof(DhtTempHumiditySensor), e);
                return null;
            }
        }
    }
}
