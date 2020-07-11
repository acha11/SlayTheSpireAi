using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SlayTheSpireAi
{
    public enum MonsterIntents
    {
        Attack,
        Buff,
        Debuff,
        Defend_Buff,
        Attack_Defend,
        Attack_Debuff,
        // I saw this once on a Cultist on the first floor, first turn
        Debug
    }

    public class Monster
    {
        public bool IntendsToAttack
        {
            get
            {
                return
                    //Intent == MonsterIntents.Attack ||
                    //Intent == MonsterIntents.Attack_Defend ||
                    //Intent == MonsterIntents.Attack_Debuff ||
                    // Not using intent because I've seen "DEBUG" come back as the monster intent.
                    (MoveHits > 0 && MoveAdjustedDamage > 0);
            }
        }

        [JsonProperty(PropertyName = "is_gone")]
        public bool IsGone { get; set; }

        [JsonProperty(PropertyName = "move_hits")]
        public int MoveHits { get; set; }

        [JsonProperty(PropertyName = "move_base_damage")]
        public int MoveBaseDamage { get; set; }

        [JsonProperty(PropertyName = "move_adjusted_damage")]
        public int MoveAdjustedDamage { get; set; }

        public MonsterIntents Intent { get; set; }
        public string Name { get; set; }

        [JsonProperty(PropertyName = "current_hp")]
        public int CurrentHp{ get; set; }

        public int Block { get; set; }

    }
}