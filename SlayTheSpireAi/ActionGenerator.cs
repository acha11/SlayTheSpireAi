using System;
using System.Collections.Generic;
using System.Text;

namespace SlayTheSpireAi
{
    public interface IAction
    {
        GameState Apply(GameState gameState);
    }

    public class EndTurnAction : IAction
    {
        public GameState Apply(GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayCardAction : IAction
    {
        public PlayCardAction(Card card)
        {
            Card = card;
        }

        public Card Card { get; }

        public GameState Apply(GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class ActionGenerator
    {
        public IAction[] GenerateActions(GameState gameState)
        {
            List<IAction> actions = new List<IAction>();

            // Can always end turn
            actions.Add(new EndTurnAction());

            // Can play any held card if we have sufficient energy.
            // (One day this will change for velvet choker, etc.)
            foreach (var card in gameState.CombatState.Hand)
            {
                if (card.Cost <= gameState.CombatState.Player.Energy)
                {
                    actions.Add(new PlayCardAction(card));
                }
            }

            return actions.ToArray();
        }
    }
}
