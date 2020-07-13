using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlayTheSpireAi
{
    public interface IAction
    {
        GameState ApplyTo(ILogger logger, GameState gameState);
        ICommand ConvertToCommand(GameState gameState);
    }

    public class EndTurnAction : IAction
    {
        public override string ToString()
        {
            return "End turn";
        }

        public GameState ApplyTo(ILogger logger, GameState gameState)
        {
            // Doesn't really make sense for the end turn action to know everything about
            // monsters attacking, etc., but let's do it here for now.

            var gs = gameState.Clone();

            // Loop through each monster and do their attacks, I guess...?
            foreach (var monster in gameState.CombatState.Monsters)
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

            return gs;
        }

        public ICommand ConvertToCommand(GameState gameState)
        {
            return new EndCommand();
        }
    }

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

        public GameState ApplyTo(ILogger logger, GameState gameState)
        {
            var gs = gameState.Clone();

            switch (Card.Id)
            {
                case "Defend_R":
                    ApplyDefendToGs(gs);
                    break;

                case "Strike_R":
                    ApplyStrikeToGs(gs);
                    break;

                case "Bash":
                    ApplyBashToGs(gs);
                    break;

                default:
                    logger.Log($"Unsupported card type '{Card.Name}'");

                    break;
            }

            gs.CombatState.Player.Energy -= Card.Cost;
            Discard(gs);

            return gs;
        }

        void ApplyDefendToGs(GameState gs)
        {
            gs.CombatState.Player.Block += 5;

            foreach (var monster in gs.CombatState.Monsters)
            {
                int rage = monster.LevelOfPower("Anger");

                if (rage > 0)
                {
                    IncreasePower(monster.Powers, "Strength", rage);
                }
            }

            Discard(gs);
        }

        void Discard(GameState gs)
        {
            gs.CombatState.Hand =
                gs.CombatState.Hand
                .Where(x => x.Uuid != Card.Uuid)
                .ToArray();
        }

        void ApplyStrikeToGs(GameState gs)
        {
            var monster = gs.CombatState.Monsters[Target.Value];

            DealAttackDamageToMonster(gs.CombatState, monster, 6);
        }

        void ApplyBashToGs(GameState gs)
        {
            var monster = gs.CombatState.Monsters[Target.Value];

            DealAttackDamageToMonster(gs.CombatState, monster, 8);

            ApplyVulnerableToMonster(monster);
        }

        void ApplyVulnerableToMonster(Monster monster)
        {
            IncreasePower(monster.Powers, "Vulnerable", 2);
        }

        void IncreasePower(List<Power> powers, string powerId, int amount)
        {
            var power = powers.SingleOrDefault(x => x.Id == powerId);

            if (power == null)
            {
                power = new Power() { Id = powerId, Name = powerId, Amount = 0 };
                powers.Add(power);
            }

            power.Amount += amount;
        }

        static void DealAttackDamageToMonster(CombatState cs, Monster monster, int baseDamage)
        {
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
                }
            }

            if (monster.CurrentHp < 0) monster.CurrentHp = 0;

            if (monster.CurrentHp == 0) monster.IsGone = true;
        }

        public ICommand ConvertToCommand(GameState gameState)
        {
            var card = gameState.CombatState.Hand.SingleOrDefault(x => x.Uuid == Card.Uuid);

            return new PlayCommand(Array.IndexOf(gameState.CombatState.Hand, card) + 1, Target);
        }
    }

    public class ActionGenerator
    {
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
            // (One day this will change for velvet choker, etc.)
            foreach (var card in gameState.CombatState.Hand)
            {
                if (card.Cost > gameState.CombatState.Player.Energy) continue;

                // Can be false if entangled. Might also reflect whether the player has sufficient energy.
                if (!card.IsPlayable) continue;

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
