This project builds a Windows command-line executable which communicates with a local instance of Slay the Spire via 
stdin and stdout. [CommunicationMod](https://github.com/ForgottenArbiter/CommunicationMod) for Slay the Spire must be
installed and active.

# Setup

The notes below are for the Windows Steam release of Slay the Spire.

1. Install Slay the Spire
2. In steam, use the "Workshop" tab for Slay the Spire to subscribe to the following mods:
	a. [ModTheSpire](https://steamcommunity.com/sharedfiles/filedetails/?id=2131373661&searchtext=communication+mod)
	b. [BaseMod](https://steamcommunity.com/sharedfiles/filedetails/?id=2131373661&searchtext=communication+mod)
	c. [Communication Mod](https://steamcommunity.com/workshop/browse/?appid=646570&searchtext=communication+mod&childpublishedfileid=0&browsesort=textsearch&section=items)
3. Launch Slay the Spire from Steam. When asked, choose "Play With Mods" rather than the default "Play Slay the Spire" option. The ModTheSpire UI should display. 
4. Check the boxes next to both BaseMod and CommunicationMod, then click "Play". The game should launch, and the main menu should be shown.
5. Modify %LOCALAPPDATA%\ModTheSpire\CommunicationMod\config.properties, setting the "command" value to the full path to the exe built by this project. Use forward slashes rather than backslashes (e.g. "command=command=C:/repos/SlayTheSpireAi/SlayTheSpireAi/bin/Debug/netcoreapp3.1/slayTheSpireAi.exe")
6. Click the main menu option "Mods". A list of mods (at least BaseMod and Communication Mod should be listed) will be shown.
7. Click "Communication Mod". Click "Config".
8. Tick the "Start external process" button.

# Helpers & diagnostics

## CardsSeen folder

The AI records the cards that it sees during the run in two subfolders of the SlayTheSpire exe's folder: CardsSeen/Implemented, and CardsSeen/Unimplemented.

## RunHistory folder

The AI creates a json file in the RunHistory folder for each run it attempts. The json file records summary-level information for each event (combat/shop/other)
during the run.

# Architecture

## class Ai

Main class.

## interface IGameConnection, implementation StdioGameConnection

Interface & implementation allowing AI to communicate with a running instance of the game.

Broken out into an interface to facilitate mocking/alternative implementations for automated testing, or connection to an alternative game engine implementation down the track.

## interface ICommand

A command that can be issued by a Slay the Spire player. Implementations are provided for all possible commands.

Implementations include:

* Choose
* End (turn)
* Play (card)
* Proceed
* Return
* Start
* Wait

## CardImplementations

interface ICardImplementation exposes two members: float BaseUtility, which guides card selection in the metagame, and ApplyCard(), which "plays" the card
against a gamestate object.

class CardImplementationBase implements ICardImplementation, but without providing much of value - I don't remember why I included it in the hierarchy.

Card implementations then inherit from CardImplementationBase.

## Cards class

Knows which cards are implemented, and how to map from the internal string id that uniquely identifies a card type to the AI's corresponding CardImplementation.

## ActionGenerator

This class has one method, GenerateActions(), which returns a list of the actions permitted given a provided GameState.

## Game states

GameState is a class that represents the total state of the game at a moment in time.

GameState has a Clone() method which returns a deep copy of itself (by serializing and deserializing). This is called a lot & is expensive. *Good candidate for optimisation, particularly when we rewrite to explore options more deeply*.

ScoredGameState wraps GameState, adding:

* a "precondition" - the player move that led to this GameState from its predecessor game state. 
* a "score" - quantifies the utility of this GameState - the higher the score, the more preferable this game state is. Moves are chosen in the direction of higher score.
* a list of child ScoredGameStates that can be reached from this GameState by playing available moves.
* a "BestScoreOfLeafNodes" recording the highest score reachable from this gamestate by playing through to the end of combat.

## PlannedStrategy

Is a method in Ai.

It executes a combat.

* PlannedStrategy()
  * EvaluateActionsUnderGameState()
  * FindActionWithBestSubscore()
  * Issues a command to take the action that leads to the best available state (which the previous two steps identified)
  * Validate the resulting game state against the expectation (this verifies that the AI's implementation of the action matches the game's implementation. Only works for deterministic actions.)