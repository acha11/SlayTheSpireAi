using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.GameLogic.ActionGenerator;
using SlayTheSpireAi.Common.GameLogic.ActionImplementations;
using SlayTheSpireAi.Common.StateRepresentations;
using System.Linq;

namespace SlayTheSpireAi.UnitTests
{
    [TestClass]
    public class ActionGeneratorTests
    {
        GameState BuildBaseGameState()
        {
            return
                new GameState()
                {
                    CombatState = new CombatState()
                    {
                        Player = new PlayerState()
                        {
                            Energy = 1
                        },
                        Hand =
                            new CardState[]
                            {
                            }
                    }
                };
        }

        [TestMethod]
        public void WhenGameStateEmptyCanOnlyEndTurn()
        {
            var cards = new Cards();

            ActionGenerator ag = new ActionGenerator(cards);

            var actions = ag.GenerateActions(BuildBaseGameState());

            var endTurnAction = actions.OfType<EndTurnAction>().Single();

            Assert.AreEqual(1, actions.Length);
        }

        [TestMethod]
        public void WhenHoldingOneCardCanPlayCard()
        {
            var cards = new Cards();

            ActionGenerator ag = new ActionGenerator(cards);

            var gameState = BuildBaseGameState();

            gameState.CombatState.Hand =
                new CardState[]
                {
                    new CardState()
                    {
                        Cost = 1,
                        IsPlayable = true,
                        Name = "Defend"
                    }
                };

            var actions = 
                ag.GenerateActions(gameState);

            var endTurnAction = actions.OfType<EndTurnAction>().Single();

            var playCardAction = actions.OfType<PlayCardAction>().Single();

            Assert.AreEqual(2, actions.Length);
        }

        [TestMethod]
        public void CannotPlayCardWithoutSufficientEnergy()
        {
            var cards = new Cards();

            ActionGenerator ag = new ActionGenerator(cards);

            var gameState = BuildBaseGameState();

            gameState.CombatState.Hand =
                new CardState[]
                {
                    new CardState()
                    {
                        Cost = 2,
                        IsPlayable = true,
                        Name = "BigDefend"
                    }
                };

            var actions =
                ag.GenerateActions(gameState);

            var endTurnAction = actions.OfType<EndTurnAction>().Single();

            Assert.AreEqual(0, actions.OfType<PlayCardAction>().Count());

            Assert.AreEqual(1, actions.Length);
        }
    }
}
