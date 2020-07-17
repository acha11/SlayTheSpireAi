using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlayTheSpireAi.Common.GameLogic;
using SlayTheSpireAi.Common.GameLogic.ActionImplementations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SlayTheSpireAi
{
    public class Ai
    {
        ILogger _logger;
        GameStateMessage _lastGameStateMessage;
        CombatState _combatState;
        Cards _cardImplementations = new Cards();
        ActionGenerator _actionGenerator;

        public Ai(ILogger logger)
        {
            _logger = logger;
            _actionGenerator = new ActionGenerator(_cardImplementations);
        }

        public void Run()
        {
            _logger.Log("");
            _logger.Log("Starting run");
            _logger.Log("------------");

            // Indicate that we're ready
            SendRaw("ready");

            Debugger.Launch();

            Send(new StartCommand("ironclad", 1, "AAA"));

            if (_lastGameStateMessage?.GameState?.ChoiceList != null)
            {
                Send(new ChooseCommand(choiceName: "talk"));

                // Options when you're a new character who didn't reach a boss: 
                //  "enemies in your next three combats have 1 hp",
                //  "max hp +7"
                //ExpectAndChooseChoice(NeowEventChoices.MaxHpPlus7);
                ExpectAndChooseChoice("obtain 100 gold");

                // After choosing, we're offered just the one option: leave
                ExpectAndChooseChoice("leave");

                while (_lastGameStateMessage.InGame)
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

                // Run is over
                _logger.Log("Run over.");
            }
        }

        void HandleEventScreen()
        {
            _logger.Log("Event");

            switch (_lastGameStateMessage.GameState.ScreenState.EventId)
            {
                case "The Cleric":
                    HandleEventTheCleric();

                    break;

                default:
                    HandleUnimplementedEvent();

                    break;
            }
        }

        void HandleEventTheCleric()
        {
            // Options are like
            //    "Heal": "[Heal] 35 Gold: Heal 22 HP."
            //    "Purify": "[Purify] 50 Gold: Remove a card from your deck."
            //    "Leave": "[Leave]"
            var healScore = CalculateUtilityOfHeal(22);

            var removeCardScore = CalculateUtilityOfRemoveCard();

            if (healScore > removeCardScore)
            {
                Send(new ChooseCommand(null, "Heal"));
            }
            else
            {
                Debugger.Launch();

                Send(new ChooseCommand(null, "Purify"));
            }

            Send(new ProceedCommand());
        }

        float CalculateUtilityOfRemoveCard()
        {
            return 30;
        }

        float CalculateUtilityOfHeal(int hp)
        {
            return hp;
        }

        void HandleUnimplementedEvent()
        {
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

                PlannedStrategy();
            }

            // Handle reward (or death?)
            if (_lastGameStateMessage.GameState.RoomPhase == "COMPLETE")
            {
                // Win. Take gold.
                Send(new ChooseCommand(choiceName: "gold"));

                // Check the card options
                Send(new ChooseCommand(choiceName: "card"));

                ChooseAmongstOfferedCards();
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

        void ChooseAmongstOfferedCards()
        {
            Thread.Sleep(2500);

            Send(new ChooseCommand(choiceIndex: 0));

            Send(new ProceedCommand());
        }

        void PlannedStrategy()
        {
            var gsw = new GameStateWrapper(_lastGameStateMessage.GameState, _cardImplementations);

            var sgs = EvaluateActionsUnderGameState(_actionGenerator, gsw, 0);

            var action = FindActionWithBestSubscore(sgs, 1, "");

            _logger.Log("Playing " + JsonConvert.SerializeObject(action.Precondition));

            Send(action.Precondition.ConvertToCommand(gsw));

            if (_lastGameStateMessage.GameState.CombatState != null)
            {
                // Check monster state against expectations
                var expectedCs = action.GameState.CombatState;
                var actualCs = _lastGameStateMessage.GameState.CombatState;

                for (int i = 0; i < action.GameState.CombatState.Monsters.Length; i++)
                {
                    var expected = expectedCs.Monsters[i];
                    var actual = actualCs.Monsters[i];

                    if (expected.CurrentHp != actual.CurrentHp)
                    {
                        _logger.Log($"Expected outcome discrepancy for monster {i}: Expected {expected.CurrentHp} HP, actual {actual.CurrentHp} HP");
                    }
                }

                // Check player state against expectations
                if (!(action.Precondition is EndTurnAction) && expectedCs.Player.Block != actualCs.Player.Block)
                {
                    _logger.Log($"Expected outcome discrepancy for player: Expected {expectedCs.Player.Block} block, actual {actualCs.Player.Block} block");
                }
            }
        }

        public ScoredGameState FindActionWithBestSubscore(List<ScoredGameState> sgs, int depth, string ancestry)
        {
            float bestScoreSoFar = float.MinValue;
            ScoredGameState bestSgsSoFar = null;

            foreach (var gs in sgs)
            {
                float score = 0;

                if (!gs.Children.Any())
                {
                    _logger.Log(gs.Score + ": " + ancestry + " " + gs.Precondition);

                    score = gs.Score;
                }
                else
                {
                    string newAncestry = ancestry.Length == 0 ? gs.Precondition.ToString() : ancestry + " " + gs.Precondition;

                    score = FindActionWithBestSubscore(gs.Children, depth + 1, newAncestry).BestScoreOfLeafNodes;
                }

                gs.BestScoreOfLeafNodes = score;

                if (score > bestScoreSoFar)
                {
                    bestScoreSoFar = score;
                    bestSgsSoFar = gs;
                }
            }
            
            return bestSgsSoFar;
        }

        public class ScoredGameState
        {
            public IAction Precondition { get; set; }
            public GameState GameState { get; set; }
            public float Score { get; set; }

            public List<ScoredGameState> Children { get; set; }
            public float BestScoreOfLeafNodes { get; internal set; }
        }

        public List<ScoredGameState> EvaluateActionsUnderGameState(ActionGenerator ag, GameStateWrapper gameStateWrapper, int depth)
        {
            var scoredGameStates = new List<ScoredGameState>();

            var actions = ag.GenerateActions(gameStateWrapper.GameState);

            foreach (var action in actions)
            {
                var gswClone = gameStateWrapper.Clone();

                action.ApplyTo(_logger, gswClone);

                var sgs =
                    new ScoredGameState()
                    {
                        Precondition = action,
                        GameState = gswClone.GameState,
                        Score = ScoreGameState(gswClone.GameState),
                        Children = new List<ScoredGameState>()
                    };

                _logger.Log("!".PadLeft(3 * depth) + "Action " + sgs.Precondition.ToString() + ": " + sgs.Score);

                if (!(action is EndTurnAction))
                {
                    var children = EvaluateActionsUnderGameState(ag, gswClone, depth + 1);

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

            // Prefer solutions that lose less energy. This is a way to reduce playing of
            // redundant cards (e.g. defends on a turn where the monster won't get to
            // attack anyway because it's going to die). Also, being lazy is stylish.
            score += result.CombatState.Player.Energy;

            // Score 20 for each defeated monster
            score += 20 * result.CombatState.Monsters.Where(x => x.CurrentHp == 0).Count();

            // Score -bignumber for dead player
            if (result.CombatState.Player.CurrentHp == 0)
            {
                score -= 1000000;
            }

            const float ValueOfOnePlayerHp = 20;

            score += result.CombatState.Player.CurrentHp * ValueOfOnePlayerHp;

            score -= result.CombatState.Monsters.Where(x => !x.IsGone).Sum(x => x.CurrentHp);

            score -= result.CombatState.Hand.Count(x => x.Id == "Slimed") * 3;

            // penalise score based on monster strength times the number of turns we estimate it will be around for.
            const float MagicNumber_NumberOfHpWeExpectToBurnDownPerTurn = 10;

            score -= result.CombatState.Monsters.Where(x => !x.IsGone).Sum(x => x.LevelOfPower("Strength") * x.CurrentHp * ValueOfOnePlayerHp / MagicNumber_NumberOfHpWeExpectToBurnDownPerTurn);

            // Value vulnerability on monsters
            foreach (var m in result.CombatState.Monsters)
            {
                var vuln = m.Powers.SingleOrDefault(x => x.Id == "Vulnerable");

                if (vuln != null)
                {
                    if (m.CurrentHp > 6 && vuln.Amount > 0) score += 3f;
                    if (m.CurrentHp > 12 && vuln.Amount > 0) score += 3f;
                    if (m.CurrentHp > 18 && vuln.Amount > 1) score += 3f;
                }
            }

            return score;
        }

        void WaitForMonsterIntentsToBeValid()
        {
            if (_combatState?.Monsters == null) return;

            // Handle the case where we get "DEBUG" back as a monster's intent, which is associated with
            // incorrect adjusted damage
            while (_combatState.Monsters.Any(x => x.Intent == MonsterIntents.Debug))
            {
                _logger.Log("Monster has DEBUG intent - requesting updated state after a delay");
                Send(new WaitCommand(10));
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
