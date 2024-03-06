namespace Infrastructure.Services.Metric
{
    public interface IMetricService
    {
        void SendEvent(string eventName);
    }
}