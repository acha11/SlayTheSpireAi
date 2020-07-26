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
                { "Anger", new AngerImplementation() },
                { "Bash", new BashImplementation() },
                { "Cleave", new CleaveImplementation() },
                { "Clothesline", new ClotheslineImplementation() },
                { "Defend_R", new DefendImplementation() },
                { "Disarm", new DisarmImplementation() },
                { "Flex", new FlexImplementation() },
                { "heavy strike", new HeavyStrikeImplementation() },
                { "juggernaut", new JuggernautImplementation() },
                { "Perfected Strike", new PerfectedStrikeImplementation() },
                { "Reckless Charge", new RecklessChargeImplementation() },
                { "Strike_R", new StrikeImplementation() },
                { "Thunderclap", new ThunderclapImplementation() },
                { "Uppercut", new UppercutImplementation() },
                { "wild strike", new WildStrikeImplementation() },

                // Silent

                // Defect

                // Watcher

                // Colorless

                // Curse

                // Status
                { "Wound", new WoundImplementation() },
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
