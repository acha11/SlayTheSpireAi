using Newtonsoft.Json;

namespace SlayTheSpireAi
{
    public class GameState
    {
        [JsonProperty(PropertyName = "choice_list")]
        public string[] ChoiceList { get; set; }

        [JsonProperty(PropertyName = "combat_state")]
        public CombatState CombatState { get; set; }
        [JsonProperty(PropertyName = "room_phase")]
        public string RoomPhase { get; set; }

        [JsonProperty(PropertyName = "screen_type")]
        public string ScreenType { get; set; }

        public GameState Clone()
        {
            return JsonConvert.DeserializeObject<GameState>(JsonConvert.SerializeObject(this));
        }
    }
}