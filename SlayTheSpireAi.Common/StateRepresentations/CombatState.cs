using Newtonsoft.Json;
using System.Collections.Generic;

namespace SlayTheSpireAi.Common.StateRepresentations
{
    public class CombatState
    {
        [JsonProperty(PropertyName = "draw_pile")]
        public List<CardState> DrawPile { get; set; }
        [JsonProperty(PropertyName = "discard_pile")]
        public List<CardState> DiscardPile { get; set; }
        public List<CardState> Hand { get; set; }
        public PlayerState Player { get; set; }
        public MonsterState[] Monsters { get; set; }
    }
}