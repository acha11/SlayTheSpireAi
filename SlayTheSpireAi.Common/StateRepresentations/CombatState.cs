namespace SlayTheSpireAi.Common.StateRepresentations
{
    public class CombatState
    {
        public CardState[] Hand { get; set; }
        public PlayerState Player { get; set; }
        public MonsterState[] Monsters { get; set; }
    }
}