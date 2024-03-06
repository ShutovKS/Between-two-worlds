#if YG_SERVICES

using YG;

namespace Infrastructure.Services.Metric
{
    public class MetricYGService : IMetricService
    {
        public void SendEvent(MetricEventType metricEventType)
        {
            var eventName = GetEventName(metricEventType);

            if (!string.IsNullOrEmpty(eventName))
            {
                YandexMetrica.Send(metricEventType.ToString());
            }
        }

        private static string GetEventName(MetricEventType metricEventType)
        {
            string eventName;

            switch (metricEventType)
            {
                case MetricEventType.Started:
                    eventName = "started";
                    break;
                case MetricEventType.Saved:
                    eventName = "saved";
                    break;
                case MetricEventType.Load:
                    eventName = "loaded";
                    break;
                case MetricEventType.End1:
                    eventName = "end1";
                    break;
                case MetricEventType.End2:
                    eventName = "end2";
                    break;
                default:
                    return null;
            }

            return eventName;
        }
    }
}

#endif