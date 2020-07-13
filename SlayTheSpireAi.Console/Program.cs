﻿using Newtonsoft.Json;
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

            var evaluation = ai.EvaluateActionsUnderGameState(new ActionGenerator(), gsm.GameState, 0);

            var chosenAction = ai.FindActionWithBestSubscore(evaluation, 0, "");
        }
    }
}
