using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                            new Card[]
                            {
                            }
                    }
                };
        }

        [TestMethod]
        public void WhenGameStateEmptyCanOnlyEndTurn()
        {
            ActionGenerator ag = new ActionGenerator();

            var actions = ag.GenerateActions(BuildBaseGameState());

            var endTurnAction = actions.OfType<EndTurnAction>().Single();

            Assert.AreEqual(1, actions.Length);
        }

        [TestMethod]
        public void WhenHoldingOneCardCanPlayCard()
        {
            ActionGenerator ag = new ActionGenerator();

            var gameState = BuildBaseGameState();

            gameState.CombatState.Hand =
                new Card[]
                {
                    new Card()
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
            ActionGenerator ag = new ActionGenerator();

            var gameState = BuildBaseGameState();

            gameState.CombatState.Hand =
                new Card[]
                {
                    new Card()
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
