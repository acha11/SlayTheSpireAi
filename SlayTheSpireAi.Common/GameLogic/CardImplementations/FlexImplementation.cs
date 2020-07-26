using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic.CardImplementations
{
    public class FlexImplementation : CardImplementationBase
    {
        public override float BaseUtility => 1.7f;

        public override void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target)
        {
            gameStateWrapper.AdjustPlayerPower(Powers.Strength, 2);
            gameStateWrapper.AdjustPlayerPower(Powers.StrengthDown, 2);
        }
    }
}
