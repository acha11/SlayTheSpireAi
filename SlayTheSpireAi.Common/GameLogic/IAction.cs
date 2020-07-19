using SlayTheSpireAi.Common.Commands;
using SlayTheSpireAi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic
{
    public interface IAction
    {
        void ApplyTo(ILogger logger, GameStateWrapper gameStateWrapper);
        ICommand ConvertToCommand(GameStateWrapper gameStateWrapper);
    }
}
