using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Commands
{
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

}
