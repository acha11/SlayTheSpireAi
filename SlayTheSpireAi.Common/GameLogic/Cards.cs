using System;
using System.Collections.Generic;

using SlayTheSpireAi.Common.CardImplementations;
using SlayTheSpireAi.Common.GameLogic.CardImplementations;

namespace SlayTheSpireAi.Common.GameLogic
{
    public class Cards
    {
        Dictionary<string, ICardImplementation> _cardImplementations =
            new Dictionary<string, ICardImplementation>(StringComparer.InvariantCultureIgnoreCase)
            {
                // Ironclad
                { "Defend_R", new DefendImplementation() },
                { "Strike_R", new StrikeImplementation() },
                { "Bash", new BashImplementation() },
                { "Cleave", new CleaveImplementation() },
                { "Flex", new FlexImplementation() },
                { "Perfected Strike", new PerfectedStrikeImplementation() },
                { "Reckless Charge", new RecklessChargeImplementation() },
                { "Thunderclap", new ThunderclapImplementation() },
                { "Disarm", new DisarmImplementation() },
                { "Clothesline", new ClotheslineImplementation() },
                { "Uppercut", new UppercutImplementation() },

                // Silent

                // Defect

                // Watcher

                // Colorless

                // Curse

                // Status

                { "Slimed", new SlimedImplementation() },
            };

        public ICardImplementation GetCardImplementationOrNull(string cardId)
        {
            if (_cardImplementations.TryGetValue(cardId, out var impl))
            {
                return impl;
            }
            else
            {
                return null;
            }
        }
    }
}
