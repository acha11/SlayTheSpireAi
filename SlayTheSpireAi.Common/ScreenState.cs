using Newtonsoft.Json;

namespace SlayTheSpireAi
{
    public class ScreenState
    {
        [JsonProperty(PropertyName = "event_id")]
        public string EventId { get; set; }
    }
}