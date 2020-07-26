namespace SlayTheSpireAi.Common.Journaling
{
    public class RunEvent
    {
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }

        public string Monsters { get; set; }
        public string[] OfferedCards { get; internal set; }
        public string TakenCard { get; internal set; }
    }
}