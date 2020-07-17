using SlayTheSpireAi.Common.CardImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic.ActionImplementations
{
    public class PlayCardAction : IAction
    {
        public PlayCardAction(Card card)
        {
            Card = card;
        }

        public Card Card { get; }
        public int? Target { get; set; }

        public override string ToString()
        {
            if (Target == null)
            {
                return $"{Card.Name}";
            }

            return $"{Card.Name} {Target}";
        }

        public void ApplyTo(ILogger logger, GameStateWrapper gameStateWrapper)
        {
            var impl = gameStateWrapper.CardImplementations.GetCardImplementationOrNull(Card.Id);

            if (impl != null)
            {
                impl.ApplyCard(Card, gameStateWrapper, Target);
            }
            else
            {
                logger.Log($"Unsupported card type '{Card.Name}'");
            }

            gameStateWrapper.GameState.CombatState.Player.Energy -= Card.Cost;

            if (Card.Exhausts)
            {
                gameStateWrapper.Exhaust(Card);
            }
            else
            {
                gameStateWrapper.Discard(Card);
            }
        }

        public ICommand ConvertToCommand(GameStateWrapper gameStateWrapper)
        {
            var gs = gameStateWrapper.GameState;

            var card = gs.CombatState.Hand.SingleOrDefault(x => x.Uuid == Card.Uuid);

            return new PlayCommand(Array.IndexOf(gs.CombatState.Hand, card) + 1, Target);
        }
    }
}
