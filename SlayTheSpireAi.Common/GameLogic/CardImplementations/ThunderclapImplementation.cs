using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.StateRepresentations;
using System.Linq;

namespace SlayTheSpireAi.Common.CardImplementations
{
    public class ThunderclapImplementation : CardImplementationBase
    {
        public override float BaseUtility => 1.5f;

        public override void ApplyCard(CardState card, GameStateWrapper gameStateWrapper, int? target)
        {
            gameStateWrapper.DealAttackDamageToAllMonsters(4);

            foreach (var monster in gameStateWrapper.GameState.CombatState.Monsters.Where(x => !x.IsGone))
            { 
                gameStateWrapper.ApplyVulnerableToMonster(monster);
            }
        }
    }
}
