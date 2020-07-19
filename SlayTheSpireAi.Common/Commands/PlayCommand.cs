using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.Commands
{
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
}
