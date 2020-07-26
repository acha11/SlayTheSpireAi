﻿using SlayTheSpireAi.Common.StateRepresentations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SlayTheSpireAi.Common.GameLogic
{
    public class GameStateWrapper
    {
        MonsterState[] _liveMonsters;
        Random _random = new Random();

        public GameStateWrapper(GameState gameState, Cards cardImplementations)
        {
            GameState = gameState;
            CardImplementations = cardImplementations;

            _liveMonsters = gameState.CombatState.Monsters.Where(x => !x.IsGone).ToArray();
        }

        public GameState GameState { get; }

        public Cards CardImplementations { get; }

        public PlayerState PlayerState { get { return GameState?.CombatState?.Player; } }

        public MonsterState[] LiveMonsters { get { return _liveMonsters; } }

        public void ShuffleCardIntoDrawPile(CardState cardState)
        {
            // I think this should be inserting at random location, but I'm not 
            // implementing draw pile order-sensitive lookahead (because that
            // only matters when you have frozen orb/frozen eye/whatever it's
            // called), and the bang for buck is not there, so I'll just put it
            // at the start of the draw pile.
            GameState.CombatState.DrawPile.Insert(0, cardState);
        }

        public int GetPlayerPower(string powerId)
        {
            return PlayerState.AmountOfPower(powerId);
        }

        public void Discard(CardState card)
        {
            // Remove card from hand. Should also put it in the discard pile.
            GameState.CombatState.Hand =
                GameState.CombatState.Hand
                .Where(x => x.Uuid != card.Uuid)
                .ToList();
        }

        public void EndTurn()
        {
            var strengthDown = PlayerState.AmountOfPower(Powers.StrengthDown);

            if (strengthDown != 0)
            {
                AdjustPlayerPower(Powers.Strength, -strengthDown);

                AdjustPlayerPower(Powers.StrengthDown, -strengthDown);
            }

            // Loop through each monster and do their attacks, I guess...?
            foreach (var monster in GameState.CombatState.Monsters)
            {
                if (monster.IsGone) continue;

                switch (monster.Intent)
                {
                    case MonsterIntents.Attack:
                    case MonsterIntents.Attack_Debuff:
                    case MonsterIntents.Attack_Defend:
                        for (int i = 0; i < monster.MoveHits; i++)
                        {
                            var block = GameState.CombatState.Player.Block;
                            var attack = monster.MoveAdjustedDamage;

                            var dmgBlocked = Math.Min(block, attack);

                            attack -= dmgBlocked;

                            GameState.CombatState.Player.Block -= dmgBlocked;

                            var newHP = GameState.CombatState.Player.CurrentHp;

                            newHP -= attack;

                            newHP = Math.Max(newHP, 0);

                            GameState.CombatState.Player.CurrentHp = newHP;

                            if (newHP == 0)
                            {
                                break;
                            }
                        }

                        break;
                }
            }
        }

        public void Exhaust(CardState card)
        {
            // Remove card from hand. Should also put it in the exhaust list.
            GameState.CombatState.Hand =
                GameState.CombatState.Hand
                .Where(x => x.Uuid != card.Uuid)
                .ToList();
        }

        public GameStateWrapper Clone()
        {
            return new GameStateWrapper(GameState.Clone(), CardImplementations);            
        }

        public void GivePlayerBlock(int delta)
        {
            GameState.CombatState.Player.Block += delta;

            var juggernautLevel = PlayerState.AmountOfPower(Powers.Juggernaut);

            if (GameState.CombatState.Player.HasPower(Powers.Juggernaut))
            {
                GameState.Deterministic = false;

                MonsterState monster = ChooseRandomMonster();

                DealAttackDamageToMonster(monster, juggernautLevel);
            }
        }

        MonsterState ChooseRandomMonster()
        {
            return _liveMonsters[_random.Next(_liveMonsters.Length)];
        }

        public void ApplyPowerToMonster(MonsterState monster, string powerId, int delta)
        {
            AdjustPower(monster.Powers, powerId, delta);
        }

        public void AdjustPlayerPower(string powerId, int delta)
        {
            AdjustPower(GameState.CombatState.Player.Powers, powerId, delta);
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

        public void DealAttackDamageToAllMonsters(int baseDamage)
        {
            foreach (var monster in GameState.CombatState.Monsters.Where(x => !x.IsGone))
            {
                DealAttackDamageToMonster(monster, baseDamage);
            }
        }

        public void DealAttackDamageToMonster(MonsterState monster, int baseDamage)
        {
            var cs = GameState.CombatState;

            var adjustedDamage = baseDamage;

            adjustedDamage += cs.Player.AmountOfPower(Powers.Strength);

            if (cs.Player.HasPower(Powers.Weakened))
            {
                adjustedDamage = (int)(adjustedDamage * 0.75);
            }

            if (monster.LevelOfPower(Powers.Vulnerable) > 0)
            {
                adjustedDamage = (int)(adjustedDamage * 1.5);
            }

            if (monster.Block > 0)
            {
                var amountBlocked = Math.Min(monster.Block, adjustedDamage);

                monster.Block -= amountBlocked;
                adjustedDamage -= amountBlocked;
            }

            var sharpHide = monster.LevelOfPower(Powers.SharpHide);

            if (sharpHide > 0)
            {
                DealDamageToPlayer(sharpHide);
            }

            if (adjustedDamage > 0)
            {
                monster.CurrentHp -= adjustedDamage;

                if (monster.LevelOfPower(Powers.CurlUp) > 0)
                {
                    monster.Block += monster.LevelOfPower(Powers.CurlUp);

                    AdjustPower(monster.Powers, Powers.CurlUp, -1);
                }
            }

            if (monster.CurrentHp < 0) monster.CurrentHp = 0;

            if (monster.CurrentHp == 0) monster.IsGone = true;
        }

        void DealDamageToPlayer(int amount)
        {
            GameState.CombatState.Player.CurrentHp = Math.Max(0, GameState.CombatState.Player.CurrentHp);
        }
    }
}
