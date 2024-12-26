using System.Text.Json;

namespace DragonAPI.IntegrationEvent.Events
{
    public class IntegrationMessageETO
    {
        public string EventName { get; set; }
        public object Payload { get; set; }
    }
}