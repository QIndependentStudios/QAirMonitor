namespace QAirMonitor.Abstract.Business
{
    public interface ISensorReadingReceivedEventArgs<TReading>
    {
        TReading NewReading { get; }
        int Attempts { get; }
    }
}
