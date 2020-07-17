using Newtonsoft.Json;

namespace SlayTheSpireAi.Common.StateRepresentations
{
    public class GameStateMessage
    {
        [JsonProperty(PropertyName = "available_commands")]
        public string[] AvailableCommands { get; set; }

        [JsonProperty(PropertyName = "game_state")]
        public GameState GameState { get; set; }

        [JsonProperty(PropertyName = "in_game")]
        public bool InGame { get; set; }

        public DeckState Deck { get; set; }
    }
}