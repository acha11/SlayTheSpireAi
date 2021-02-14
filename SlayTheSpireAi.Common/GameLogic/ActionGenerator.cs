using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.GameLogic.ActionImplementations;
using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic.ActionGenerator
{
    public class ActionGenerator
    {
        Cards _cards;

        public ActionGenerator(Cards cards)
        {
            _cards = cards;
        }

        /// <summary>
        /// Returns a list of the actions permitted starting at the supplied
        /// game state.
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        public IAction[] GenerateActions(GameState gameState)
        {
            List<IAction> actions = new List<IAction>();

            // Can always end turn
            actions.Add(new EndTurnAction());

            // Can play any held card if we have sufficient energy.
            // (One day this will change for velvet choker (prevents playing more than 6 cards per turn), etc.)
            foreach (var card in gameState.CombatState.Hand)
            {
                if (card.Cost > gameState.CombatState.Player.Energy) continue;

                // Can be false if entangled. Might also reflect whether the player has sufficient energy.
                if (!card.IsPlayable) continue;

                // If the AI doesn't currently understand this card, ignore it
                if (_cards.GetCardImplementationOrNull(card.Id) == null) continue;

                // Is it a targeted card?
                if (card.HasTarget)
                {
                    // Allow it to be played targetting any of the monsters that
                    // are still here
                    for (int i = 0; i < gameState.CombatState.Monsters.Length; i++)
                    {
                        if (!gameState.CombatState.Monsters[i].IsGone)
                        {
                            actions.Add(new PlayCardAction(card) { Target = i });
                        }
                    }
                }
                else
                {
                    actions.Add(new PlayCardAction(card));
                }
            }

            return actions.ToArray();
        }
    }
}
