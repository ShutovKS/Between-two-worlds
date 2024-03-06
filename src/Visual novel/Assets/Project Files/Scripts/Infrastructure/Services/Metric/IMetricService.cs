namespace Infrastructure.Services.Metric
{
    public interface IMetricService
    {
        void SendEvent(MetricEventType metricEventType);
    }
}