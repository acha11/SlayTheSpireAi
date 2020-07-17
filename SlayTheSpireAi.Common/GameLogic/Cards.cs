using System.Collections.Generic;

using SlayTheSpireAi.Common.CardImplementations;

namespace SlayTheSpireAi.Common.GameLogic
{
    public class Cards
    {
        Dictionary<string, ICardImplementation> _cardImplementations =
            new Dictionary<string, ICardImplementation>()
            {
                { "Defend_R", new DefendImplementation() },
                { "Strike_R", new StrikeImplementation() },
                { "Bash", new BashImplementation() },
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
