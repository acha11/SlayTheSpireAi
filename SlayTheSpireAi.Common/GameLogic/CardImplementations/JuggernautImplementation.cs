using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.CardImplementations
{
    public class JuggernautImplementation : CardImplementationBase
    {
        // Temporarily high to force AI to take it so that we can document what it does
        public override float BaseUtility => 1.95f;

        public override void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target)
        {
            gameStateWrapper.AdjustPlayerPower(Powers.Juggernaut, 5);
        }
    }
}
