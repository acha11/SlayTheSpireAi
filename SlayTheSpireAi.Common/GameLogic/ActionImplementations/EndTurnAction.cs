using SlayTheSpireAi.Common.StateRepresentations;
using SlayTheSpireAi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic.ActionImplementations
{
    public class EndTurnAction : IAction
    {
        public override string ToString()
        {
            return "End turn";
        }

        public void ApplyTo(ILogger logger, GameStateWrapper gameStateWrapper)
        {
            var gs = gameStateWrapper.GameState;

            // Loop through each monster and do their attacks, I guess...?
            foreach (var monster in gs.CombatState.Monsters)
            {
                if (monster.IsGone) continue;

                switch (monster.Intent)
                {
                    case MonsterIntents.Attack:
                    case MonsterIntents.Attack_Debuff:
                    case MonsterIntents.Attack_Defend:
                        for (int i = 0; i < monster.MoveHits; i++)
                        {
                            var block = gs.CombatState.Player.Block;
                            var attack = monster.MoveAdjustedDamage;

                            var dmgBlocked = Math.Min(block, attack);

                            attack -= dmgBlocked;

                            gs.CombatState.Player.Block -= dmgBlocked;

                            var newHP = gs.CombatState.Player.CurrentHp;

                            newHP -= attack;

                            newHP = Math.Max(newHP, 0);

                            gs.CombatState.Player.CurrentHp = newHP;

                            if (newHP == 0)
                            {
                                break;
                            }
                        }

                        break;
                }
            }
        }

        public ICommand ConvertToCommand(GameStateWrapper gameStateWrapper)
        {
            return new EndCommand();
        }
    }
}
