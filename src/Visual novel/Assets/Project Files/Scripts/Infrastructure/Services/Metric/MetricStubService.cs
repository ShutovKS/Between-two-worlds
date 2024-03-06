using UnityEngine;

namespace Infrastructure.Services.Metric
{
    public class MetricStubService : IMetricService
    {
        public void SendEvent(MetricEventType metricEventType)
        {
            Debug.Log($"[Metric] SendEvent: {metricEventType.ToString()}");
        }
    }
}