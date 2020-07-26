using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.CardImplementations
{
    public class ClotheslineImplementation : CardImplementationBase
    {
        public override float BaseUtility => 1.8f;

        public override void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target)
        {
            var monster = gameStateWrapper.GameState.CombatState.Monsters[target.Value];

            gameStateWrapper.DealAttackDamageToMonster(monster, 12);

            gameStateWrapper.ApplyPowerToMonster(monster, Powers.Weakened, 2);
        }
    }
}
