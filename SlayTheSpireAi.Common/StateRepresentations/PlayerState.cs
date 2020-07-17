using Newtonsoft.Json;
using System;
using System.Linq;

namespace SlayTheSpireAi.Common.StateRepresentations
{
    public class PlayerState
    {
        public int Energy { get; set; }
        public int Block { get; set; }
        public PowerState[] Powers { get; set; }

        [JsonProperty(PropertyName = "current_hp")]
        public int CurrentHp { get; set; }

        public bool HasPower(string powerId)
        {
            return Powers.Any(x => x.Id == powerId);
        }
    }
}