using Newtonsoft.Json;
using SlayTheSpireAi.Common.StateRepresentations;
using SlayTheSpireAi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common
{
    public interface IGameConnection
    { 
        GameStateMessage Send(ICommand command);
    }

    /// <summary>
    /// Communicates (via stdin and stdout) with a local running instance of Slay The Spire that has CommunicationMod
    /// running.
    /// </summary>
    public class StdioConnectionToRealGame : IGameConnection
    {
        ILogger _logger;

        public StdioConnectionToRealGame(ILogger logger)
        {
            _logger = logger;

            SendRaw("ready");
        }

        public GameStateMessage Send(ICommand command)
        {
            return SendRaw(command.GetString());
        }

        GameStateMessage SendRaw(string command)
        {
            _logger.Log($"Sending '{command}'");

            // Actually send the command over to the game by writing to stdout
            Console.WriteLine(command);

            // Await updated state
            var updatedState = Console.ReadLine();

            _logger.Log("Received state: " + updatedState);

            GameStateMessage gsm = JsonConvert.DeserializeObject<GameStateMessage>(updatedState);

            _logger.Log("Parsed to: " + JsonConvert.SerializeObject(gsm, Formatting.Indented));

            return gsm;
        }
    }
}
