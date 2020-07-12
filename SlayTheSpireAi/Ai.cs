using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

            Send(new StartCommand("ironclad", 1, null));

            if (_lastGameStateMessage?.GameState?.ChoiceList != null)
            {
                Send(new ChooseCommand(choiceName: "talk"));

                // Options when you're a new character who didn't reach a boss: 
                //  "enemies in your next three combats have 1 hp",
                //  "max hp +7"
                ExpectAndChooseChoice(NeowEventChoices.MaxHpPlus7);

                // After choosing, we're offered just the one option: leave
                ExpectAndChooseChoice("leave");

                while (true)
                {
                    // Now, we're on the map screen & offered rooms. Just choose the first so we can get in a fight.
                    // This will fail if the first offered room happens to be a question mark.
                    Send(new ChooseCommand(choiceIndex: 0));

                    // Now, we're probably in combat.
                    switch (_lastGameStateMessage.GameState.ScreenType)
                    {
                        case "EVENT":
                            HandleEventScreen();

                            break;
                        case "NONE":
                            RunCombat();

                            break;

                        default:
                            // Shops come this way.
                            Send(new ProceedCommand());

                            break;
                    }
                }
            }
        }

        void HandleEventScreen()
        {
            _logger.Log("Event");

            if (_lastGameStateMessage.AvailableCommands.Contains("proceed"))
            {
                _logger.Log("Sending proceed");

                Send(new ProceedCommand());
            }
            else
            {
                if (_lastGameStateMessage.GameState.ChoiceList.Contains("leave"))
                {
                    _logger.Log("Choosing leave");

                    Send(new ChooseCommand(null, "leave"));
                }
                else
                {
                    _logger.Log("Choosing choice #0");

                    Send(new ChooseCommand(0));
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

            while (_lastGameStateMessage?.GameState?.RoomPhase == "COMBAT" && _lastGameStateMessage?.GameState?.ScreenType != "GAME_OVER")
            {
                WaitForMonsterIntentsToBeValid();

                _logger.Log($"Turn {++turnNumber}");

                // BlockAndStrikeStrategy();

                PlannedStrategy();
            }

            // Handle reward (or death?)
            if (_lastGameStateMessage.GameState.RoomPhase == "COMPLETE")
            {
                // Win. Take gold, ignore card.
                Send(new ChooseCommand(choiceName: "gold"));

                Send(new ProceedCommand());
            }
            else
            {
                if (_lastGameStateMessage?.GameState?.ScreenType == "GAME_OVER")
                {
                    Send(new ProceedCommand());
                }
                else
                {
                    throw new Exception("Indeterminate combat outcome.");
                }
            }
        }

        void PlannedStrategy()
        {
            ActionGenerator ag = new ActionGenerator();

            var sgs = EvaluateActionsUnderGameState(ag, _lastGameStateMessage.GameState, 0);

            var action = FindActionWithBestSubscore(sgs, 1);

            _logger.Log("Playing " + JsonConvert.SerializeObject(action.Precondition));

            Send(action.Precondition.ConvertToCommand(_lastGameStateMessage.GameState));
        }

        private ScoredGameState FindActionWithBestSubscore(List<ScoredGameState> sgs, int depth)
        {
            float bestScoreSoFar = float.MinValue;
            ScoredGameState bestSgsSoFar = null;

            foreach (var gs in sgs)
            {
                float score = 0;

                if (!gs.Children.Any())
                {
                    _logger.Log("".PadLeft(depth * 3) + gs.Precondition.ToString() + ": " + gs.Score);
                    score = gs.Score;
                }
                else
                {
                    _logger.Log("".PadLeft(depth * 3) + gs.Precondition.ToString() + ": has children");

                    score = FindActionWithBestSubscore(gs.Children, depth + 1).BestScoreOfLeafNodes;
                }

                gs.BestScoreOfLeafNodes = score;

                _logger.Log("".PadLeft(depth * 3, '!') + "scored " + score);

                if (score > bestScoreSoFar)
                {
                    _logger.Log("".PadLeft(depth * 3, '@') + "that's an improvement. " + score);

                    bestScoreSoFar = score;
                    bestSgsSoFar = gs;
                }
            }
            
            _logger.Log("".PadLeft(depth * 3, '#') + " group done. bestscore " + bestScoreSoFar);

            return bestSgsSoFar;
        }

        class ScoredGameState
        {
            public IAction Precondition { get; set; }
            public GameState GameState { get; set; }
            public float Score { get; set; }

            public List<ScoredGameState> Children { get; set; }
            public float BestScoreOfLeafNodes { get; internal set; }
        }

        List<ScoredGameState> EvaluateActionsUnderGameState(ActionGenerator ag, GameState gameState, int depth)
        {
            var scoredGameStates = new List<ScoredGameState>();

            var actions = ag.GenerateActions(gameState);

            foreach (var action in actions)
            {
                var result = action.ApplyTo(_logger, gameState);

                var sgs =
                    new ScoredGameState()
                    {
                        Precondition = action,
                        GameState = result,
                        Score = ScoreGameState(result),
                        Children = new List<ScoredGameState>()
                    };

                _logger.Log("!".PadLeft(3 * depth) + "Action " + sgs.Precondition.ToString() + ": " + sgs.Score);

                if (!(action is EndTurnAction))
                {
                    var children = EvaluateActionsUnderGameState(ag, result, depth + 1);

                    foreach (var child in children)
                    {
                        sgs.Children.Add(child);
                    }
                }

                scoredGameStates.Add(sgs);
            }

            return scoredGameStates;
        }

        private float ScoreGameState(GameState result)
        {
            float score = 0;

            // Score 20 for each defeated monster
            score += 20 * result.CombatState.Monsters.Where(x => x.CurrentHp == 0).Count();

            // S

            // Score -bignumber for dead player

            if (result.CombatState.Player.CurrentHp == 0)
            {
                score -= 1000000;
            }

            score += result.CombatState.Player.CurrentHp * 20;

            score -= result.CombatState.Monsters.Where(x => !x.IsGone).Sum(x => x.CurrentHp);

            return score;
        }

        private void BlockAndStrikeStrategy()
        {
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
