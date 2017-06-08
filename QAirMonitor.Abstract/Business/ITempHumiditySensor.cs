namespace QAirMonitor.Abstract.Business
{
    public interface ITempHumiditySensor<out TReading>
    {
        TReading GetReading();
    }
}
