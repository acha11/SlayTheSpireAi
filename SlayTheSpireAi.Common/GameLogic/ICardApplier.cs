using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic
{
    public interface ICardImplementation
    {
        void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target);
    }
}
