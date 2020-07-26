using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.CardImplementations
{
    public class DefendImplementation : CardImplementationBase
    {
        public override void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target)
        {
            gameStateWrapper.GivePlayerBlock(5);

            foreach (var monster in gameStateWrapper.GameState.CombatState.Monsters)
            {
                int rage = monster.LevelOfPower(Powers.Anger);

                if (rage > 0)
                {
                    gameStateWrapper.AdjustPower(monster.Powers, Powers.Strength, rage);
                }
            }
        }
    }
}
