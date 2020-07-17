using Newtonsoft.Json;
using System;

namespace SlayTheSpireAi.Common.StateRepresentations
{
    public class CardState
    {
        public bool Exhausts { get; set; }

        [JsonProperty(PropertyName = "is_playable")]
        public bool IsPlayable { get; set; }

        public int Cost { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public Guid Uuid { get; set; }

        [JsonProperty(PropertyName = "has_target")]
        public bool HasTarget { get; set; }
    }
}