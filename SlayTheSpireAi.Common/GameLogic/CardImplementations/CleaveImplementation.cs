using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic.CardImplementations
{
    public class CleaveImplementation : CardImplementationBase
    {
        public override float BaseUtility => 1.7f;

        public override void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target)
        {
            gameStateWrapper.DealAttackDamageToAllMonsters(8);
        }
    }
}
