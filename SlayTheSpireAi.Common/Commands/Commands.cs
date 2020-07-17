using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi
{
    public interface ICommand
    {
        string GetString();
    }

    public class CommandHelpers
    {
        public static string FormatCommandWithOptionals(string commandName, params object[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(commandName);

            foreach (var p in parameters)
            {
                if (p != null)
                {
                    sb.Append(" " + p);
                }
            }

            return sb.ToString();
        }
    }

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

    public class ChooseCommand : ICommand
    {
        public ChooseCommand(int? choiceIndex = null, string choiceName = null)
        {
            ChoiceIndex = choiceIndex;
            ChoiceName = choiceName;
        }

        public int? ChoiceIndex { get; }
        public string ChoiceName { get; }

        public string GetString()
        {
            return CommandHelpers.FormatCommandWithOptionals("CHOOSE", ChoiceIndex, ChoiceName);
        }
    }

    public class PlayCommand : ICommand
    {
        public PlayCommand(int cardIndex, int? targetIndex = null)
        {
            CardIndex = cardIndex;
            TargetIndex = targetIndex;
        }

        public int CardIndex { get; }
        public int? TargetIndex { get; }

        public string GetString()
        {
            return CommandHelpers.FormatCommandWithOptionals("PLAY", CardIndex, TargetIndex);
        }
    }

    public class EndCommand : ICommand
    {
        public string GetString()
        {
            return "END";
        }
    }

    public class ProceedCommand : ICommand
    {
        public string GetString()
        {
            return "PROCEED";
        }
    }

    public class WaitCommand : ICommand
    {
        public WaitCommand(int timeout)
        {
            Timeout = timeout;
        }

        public int Timeout { get; }

        public string GetString()
        {
            return CommandHelpers.FormatCommandWithOptionals("WAIT", Timeout);
        }
    }

}
