using Newtonsoft.Json;

namespace SlayTheSpireAi
{
    public class PlayerState
    {
        public int Energy { get; set; }
        public int Block { get; set; }
        public Power[] Powers { get; set; }

        [JsonProperty(PropertyName = "current_hp")]
        public int CurrentHp { get; set; }
    }
}