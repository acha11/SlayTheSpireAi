using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace SlayTheSpireAi
{
    public class Ai
    {
        ILogger _logger;
        GameStateMessage _lastGameStateMessage;
        CombatState _combatState;

        public Ai(ILogger logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.Log("");
            _logger.Log("Starting run");
            _logger.Log("------------");

            // Indicate that we're ready
            SendRaw("ready");

            Send(new StartCommand("silent", 1, null));

            if (_lastGameStateMessage?.GameState?.ChoiceList != null)
            {
                Send(new ChooseCommand(choiceName: "talk"));

                // Options when you're a new character who didn't reach a boss: 
                //  "enemies in your next three combats have 1 hp",
                //  "max hp +7"
                ExpectAndChooseChoice(NeowEventChoices.MaxHpPlus7);

                // After choosing, we're offered just the one option: leave
                ExpectAndChooseChoice("leave");

                // Now, we're on the map screen & offered rooms. Just choose the first so we can get in a fight.
                // This will fail if the first one happens to be a question mark.
                Send(new ChooseCommand(choiceIndex: 0));

                // Now, we're probably in combat.
                if (_lastGameStateMessage?.GameState?.RoomPhase != "COMBAT")
                {
                    throw new Exception("Not in combat. Probably the first room wasn't a combat, somehow?");
                }

                while (true)
                {
                    RunCombat();

                    switch (_lastGameStateMessage.GameState.RoomPhase)
                    {
                        case "COMPLETE":
                            // Win. Take gold, ignore card.
                            Send(new ChooseCommand(choiceName: "gold"));

                            Send(new ProceedCommand());

                            break;
                    }
                }
            }
        }

        int CalculateIncomingDamage()
        {
            return
                _combatState.Monsters
                .Where(x => !x.IsGone && x.IntendsToAttack)
                .Sum(x => x.MoveAdjustedDamage * x.MoveHits);
        }

        void RunCombat()
        {
            _logger.Log("Starting combat");

            int turnNumber = 0;

            while (_lastGameStateMessage?.GameState?.RoomPhase == "COMBAT")
            {
                WaitForMonsterIntentsToBeValid();

                _logger.Log($"Turn {++turnNumber}");

                _logger.Log("Phase 1: block until it's no longer possible or no longer necessary");

                // First, block until we are blocking more than the incoming damage, or we can't block any more, or the fight is over
                while (_combatState.Player.Energy > 0 && CalculateIncomingDamage() > _combatState.Player.Block && _lastGameStateMessage?.GameState?.RoomPhase == "COMBAT")
                {
                    _logger.Log("Looking for a block card");

                    var bestBlockCard = _combatState.Hand.FirstOrDefault(x => x.Name == "Defend" && x.Cost <= _combatState.Player.Energy);

                    if (bestBlockCard == null)
                    {
                        _logger.Log("No block card found.");

                        break;
                    }

                    _logger.Log($"Playing card {bestBlockCard.Uuid}");

                    Play(bestBlockCard);
                }

                _logger.Log("Phase 2: attack");

                // Now, attack until we can't, or the fight ends
                while (_lastGameStateMessage?.GameState?.RoomPhase == "COMBAT")
                {
                    _logger.Log("Looking for an attack card");

                    var bestAttackCard = _combatState.Hand.FirstOrDefault(x => x.Name == "Strike" && x.Cost <= _combatState.Player.Energy);

                    if (bestAttackCard == null)
                    {
                        _logger.Log("No attack card found.");

                        break;
                    }

                    _logger.Log($"Playing card {bestAttackCard.Uuid}");

                    // Choose a monster to attack
                    var target = _combatState.Monsters.FirstOrDefault(x => !x.IsGone);

                    _logger.Log($"Chose monster: " + target.Name);

                    Play(bestAttackCard, Array.IndexOf(_combatState.Monsters, target));
                }

                // If we're still in combat, end our turn
                if (_lastGameStateMessage?.GameState?.RoomPhase == "COMBAT")
                {
                    Send(new EndCommand());
                }
            }
        }

        void Play(Card card, int? targetIndex = null)
        {
            // Get the 1-based index of the card in the current hand
            var cardIndex = 1 + Array.IndexOf(_combatState.Hand, card);

            Send(new PlayCommand(cardIndex, targetIndex));

            WaitForMonsterIntentsToBeValid();
        }

        void WaitForMonsterIntentsToBeValid()
        {
            if (_combatState?.Monsters == null) return;

            // Handle the case where we get "DEBUG" back as a monster's intent, which is associated with
            // incorrect adjusted damage
            while (_combatState.Monsters.Any(x => x.Intent == MonsterIntents.Debug))
            {
                _logger.Log("Monster has DEBUG intent - requesting updated state after a delay");
                Send(new WaitCommand(60));
            }
        }

        void ExpectAndChooseChoice(string choiceName)
        {
            ExpectChoice(choiceName);

            Send(new ChooseCommand(choiceName: choiceName));
        }

        void ExpectChoice(string choiceName)
        {
            var choices = _lastGameStateMessage?.GameState?.ChoiceList;

            if (!choices.Any(x => x == choiceName))
            {
                throw new Exception($"Didn't see expected option {choiceName}.");
            }
        }

        void Send(ICommand command)
        {
            SendRaw(command.GetString());
        }

        void SendRaw(string command)
        {
            _logger.Log($"Sending '{command}'");

            Console.WriteLine(command);

            // Await updated state
            var updatedState = Console.ReadLine();

            _logger.Log("Received state: " + updatedState);

            GameStateMessage gsm = JsonConvert.DeserializeObject<GameStateMessage>(updatedState);

            _logger.Log("Parsed to: " + JsonConvert.SerializeObject(gsm, Formatting.Indented));

            _lastGameStateMessage = gsm;

            _combatState = _lastGameStateMessage.GameState?.CombatState;
        }
    }
}
