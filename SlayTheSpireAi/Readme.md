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