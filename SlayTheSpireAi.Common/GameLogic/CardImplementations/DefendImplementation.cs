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
            gameStateWrapper.GameState.CombatState.Player.Block += 5;

            foreach (var monster in gameStateWrapper.GameState.CombatState.Monsters)
            {
                int rage = monster.LevelOfPower("Anger");

                if (rage > 0)
                {
                    gameStateWrapper.AdjustPower(monster.Powers, "Strength", rage);
                }
            }
        }
    }
}
