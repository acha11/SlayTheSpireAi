using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic
{
    public class GameStateWrapper
    {
        GameState _gameState;
        Cards _cardImplementations;

        public GameStateWrapper(GameState gameState, Cards cardImplementations)
        {
            _gameState = gameState;
            _cardImplementations = cardImplementations;
        }

        public GameState GameState { get { return _gameState; } }

        public void Discard(CardState card)
        {
            // Remove card from hand. Should also put it in the discard pile.
            _gameState.CombatState.Hand =
                _gameState.CombatState.Hand
                .Where(x => x.Uuid != card.Uuid)
                .ToArray();
        }

        public void Exhaust(CardState card)
        {
            // Remove card from hand. Should also put it in the exhaust list.
            _gameState.CombatState.Hand =
                _gameState.CombatState.Hand
                .Where(x => x.Uuid != card.Uuid)
                .ToArray();
        }

        public GameStateWrapper Clone()
        {
            return new GameStateWrapper(_gameState.Clone(), _cardImplementations);            
        }

        public Cards CardImplementations { get { return _cardImplementations; } }

        public void ApplyVulnerableToMonster(MonsterState monster)
        {
            AdjustPower(monster.Powers, "Vulnerable", 2);
        }

        public void AdjustPower(List<PowerState> powers, string powerId, int delta)
        {
            var power = powers.SingleOrDefault(x => x.Id == powerId);

            if (power == null)
            {
                power = new PowerState() { Id = powerId, Name = powerId, Amount = 0 };
                powers.Add(power);
            }

            power.Amount += delta;

            if (power.Amount <= 0)
            {
                powers.Remove(power);
            }
        }

        public void DealAttackDamageToMonster(MonsterState monster, int baseDamage)
        {
            var cs = GameState.CombatState;

            var adjustedDamage = baseDamage;

            if (cs.Player.HasPower("Weakened"))
            {
                adjustedDamage = (int)(adjustedDamage * 0.75);
            }

            if (monster.LevelOfPower("Vulnerable") > 0)
            {
                adjustedDamage = (int)(adjustedDamage * 1.5);
            }

            if (monster.Block > 0)
            {
                var amountBlocked = Math.Min(monster.Block, adjustedDamage);

                monster.Block -= amountBlocked;
                adjustedDamage -= amountBlocked;
            }

            if (adjustedDamage > 0)
            {
                monster.CurrentHp -= adjustedDamage;

                if (monster.LevelOfPower("Curl Up") > 0)
                {
                    monster.Block += monster.LevelOfPower("Curl Up");

                    AdjustPower(monster.Powers, "Curl Up", -1);
                }
            }

            if (monster.CurrentHp < 0) monster.CurrentHp = 0;

            if (monster.CurrentHp == 0) monster.IsGone = true;
        }
    }
}
