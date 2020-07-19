using SlayTheSpireAi.Common.Commands;
using SlayTheSpireAi.Common.StateRepresentations;
using SlayTheSpireAi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic.ActionImplementations
{
    public class EndTurnAction : IAction
    {
        public override string ToString()
        {
            return "End turn";
        }

        public void ApplyTo(ILogger logger, GameStateWrapper gameStateWrapper)
        {
            gameStateWrapper.EndTurn();
        }

        public ICommand ConvertToCommand(GameStateWrapper gameStateWrapper)
        {
            return new EndCommand();
        }
    }
}
