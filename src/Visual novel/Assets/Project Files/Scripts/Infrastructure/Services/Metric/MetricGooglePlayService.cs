using System;
using GooglePlayGames;

namespace Infrastructure.Services.Metric
{
    public class MetricGooglePlayService : IMetricService
    {
        private static bool IsAuthenticated => PlayGamesPlatform.Instance.IsAuthenticated();
        
        public void SendEvent(MetricEventType metricEventType)
        {
            if (!IsAuthenticated)
            {
                return;
            }
            
            var eventId = GetEventId(metricEventType);
            
            PlayGamesPlatform.Instance.Events.IncrementEvent(eventId, 1);
        }

        private static string GetEventId(MetricEventType metricEventType)
        {
            return metricEventType switch
            {
                MetricEventType.Started => GPGSIds.event_launch,
                MetricEventType.Saved => GPGSIds.event_save,
                MetricEventType.Load => GPGSIds.event_load,
                MetricEventType.End1 => GPGSIds.event_end_reality,
                MetricEventType.End2 => GPGSIds.event_end_dream,
                _ => throw new ArgumentOutOfRangeException(nameof(metricEventType), metricEventType, null)
            };
        }
    }
}