﻿using SlayTheSpireAi.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi.Common.CardImplementations
{
    public class StrikeImplementation : ICardImplementation
    {
        public void ApplyCard(Card card, GameStateWrapper gameStateWrapper, int? target)
        {
            var monster = gameStateWrapper.GameState.CombatState.Monsters[target.Value];

            gameStateWrapper.DealAttackDamageToMonster(monster, 6);
        }
    }
}
