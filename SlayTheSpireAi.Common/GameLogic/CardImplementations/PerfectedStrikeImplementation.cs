﻿using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;

namespace SlayTheSpireAi.Common.GameLogic.CardImplementations
{
    public class PerfectedStrikeImplementation : CardImplementationBase
    {
        public override float BaseUtility => 1.8f;

        public override void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target)
        {
            var monster = gameStateWrapper.GameState.CombatState.Monsters[target.Value];

            gameStateWrapper.DealAttackDamageToMonster(monster, 6 + gameStateWrapper.GameState.Deck.Count(x => x.Name.Contains("Strike")) * 2);
        }
    }
}
