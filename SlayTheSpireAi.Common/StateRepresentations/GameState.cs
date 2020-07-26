using Newtonsoft.Json;
using System.Collections.Generic;

namespace SlayTheSpireAi.Common.StateRepresentations
{
    public class GameState
    {
        public GameState()
        {
            Deterministic = true;
        }

        [JsonProperty(PropertyName = "choice_list")]
        public string[] ChoiceList { get; set; }

        [JsonProperty(PropertyName = "combat_state")]
        public CombatState CombatState { get; set; }
        [JsonProperty(PropertyName = "room_phase")]
        public string RoomPhase { get; set; }

        [JsonProperty(PropertyName = "screen_type")]
        public string ScreenType { get; set; }

        public long Seed { get; set; }

        [JsonProperty(PropertyName = "screen_state")]
        public ScreenState ScreenState { get; set; }

        [JsonProperty(PropertyName = "current_hp")]
        public int CurrentHp { get; set; }
        [JsonProperty(PropertyName = "max_hp")]
        public int MaxHp { get; set; }

        public List<CardState> Deck { get; set; }
        /// <summary>
        /// Tracks whether the changes made to this game state as part of planning were deterministic
        /// </summary>
        public bool Deterministic { get; set; }

        public GameState Clone()
        {
            return JsonConvert.DeserializeObject<GameState>(JsonConvert.SerializeObject(this));
        }
    }
}