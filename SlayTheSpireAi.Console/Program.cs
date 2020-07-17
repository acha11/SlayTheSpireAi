using Newtonsoft.Json;
using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.GameLogic.ActionGenerator;
using SlayTheSpireAi.Common.StateRepresentations;
using SlayTheSpireAi.Infrastructure;
using System;
using System.IO;

namespace SlayTheSpireAi.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText(args[0]);

            var gsm = JsonConvert.DeserializeObject<GameStateMessage>(json);

            var ai = new Ai(new ConsoleLogger());

            var cards = new Cards();

            var gsw = new GameStateWrapper(gsm.GameState, cards);

            var evaluation = ai.EvaluateActionsUnderGameState(new ActionGenerator(cards), gsw, 0);

            var chosenAction = ai.FindActionWithBestSubscore(evaluation, 0, "");

            System.Console.WriteLine("Chosen action, with score " + chosenAction.BestScoreOfLeafNodes);

            System.Console.WriteLine(chosenAction.Precondition);
        }
    }
}
