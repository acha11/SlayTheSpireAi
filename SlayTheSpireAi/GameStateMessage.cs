using Newtonsoft.Json;

namespace SlayTheSpireAi
{
    public class GameStateMessage
    {
        [JsonProperty(PropertyName = "available_commands")]
        public string[] AvailableCommands { get; set; }

        [JsonProperty(PropertyName = "game_state")]
        public GameState GameState { get; set; }

    }
}