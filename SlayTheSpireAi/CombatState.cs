using System.ComponentModel.DataAnnotations;

namespace SlayTheSpireAi
{
    public class CombatState
    {
        public Card[] Hand { get; set; }

        public PlayerState Player { get; set; }
        public Monster[] Monsters { get; set; }
    }
}