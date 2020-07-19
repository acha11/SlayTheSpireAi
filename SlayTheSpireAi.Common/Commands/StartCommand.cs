using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Commands
{
    public class StartCommand : ICommand
    {
        public StartCommand(string playerClass, int? ascensionLevel, string seed)
        {
            PlayerClass = playerClass;
            AscensionLevel = ascensionLevel;
            Seed = seed;
        }

        public string PlayerClass { get; }
        public int? AscensionLevel { get; }
        public string Seed { get; }

        public string GetString()
        {
            return CommandHelpers.FormatCommandWithOptionals("START", PlayerClass, AscensionLevel, Seed);
        }
    }
}
