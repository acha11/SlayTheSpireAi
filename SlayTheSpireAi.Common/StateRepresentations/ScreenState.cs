using Newtonsoft.Json;

namespace SlayTheSpireAi.Common.StateRepresentations
{
    public class ScreenState
    {
        [JsonProperty(PropertyName = "event_id")]
        public string EventId { get; set; }
    }
}