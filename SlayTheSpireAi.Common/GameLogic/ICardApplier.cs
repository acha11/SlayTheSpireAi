using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic
{
    public interface ICardImplementation
    {
        float BaseUtility { get; }

        void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target);
    }

    public abstract class CardImplementationBase : ICardImplementation
    {
        public virtual float BaseUtility => 1;

        public abstract void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target);
    }
}
