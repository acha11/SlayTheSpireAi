using Newtonsoft.Json;
using System;

namespace SlayTheSpireAi
{
    public class Card
    {
        public bool Exhausts { get; set; }

        [JsonProperty(PropertyName = "is_playable")]
        public bool IsPlayable { get; set; }

        public int Cost { get; set; }
        public string Name { get; set; }

        public Guid Uuid { get; set; }

        [JsonProperty(PropertyName = "has_target")]
        public bool HasTarget { get; set; }
    }
}