using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic
{
    public interface ICardImplementation
    {
        void ApplyCard(Card card, GameStateWrapper gameStateWrapper, int? target);
    }
}
