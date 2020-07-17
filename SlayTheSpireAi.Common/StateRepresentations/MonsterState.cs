﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace SlayTheSpireAi.Common.StateRepresentations
{
    public enum MonsterIntents
    {
        Attack,
        Buff,
        Debuff,
        Defend,
        Defend_Buff,
        Attack_Defend,
        Attack_Debuff,
        Sleep,
        Stun,
        Strong_Debuff,
        Unknown,
        Escape,

        // "DEBUG" seems to be an interim state that the comms layer mistakenly treats as final & passes on to me
        // in some cases. When it appears, I manually request a state update and it disappears, replaced with the
        // actual intent.
        Debug
    }

    public class MonsterState
    {
        public bool IntendsToAttack
        {
            get
            {
                return
                    Intent == MonsterIntents.Attack ||
                    Intent == MonsterIntents.Attack_Defend ||
                    Intent == MonsterIntents.Attack_Debuff;
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
        public int CurrentHp { get; set; }
        [JsonProperty(PropertyName = "max_hp")]
        public int MaxHp { get; set; }

        public int Block { get; set; }
        public List<PowerState> Powers { get; set; }
        public int LevelOfPower(string powerId)
        {
            return Powers.SingleOrDefault(x => x.Id == powerId)?.Amount ?? 0;
        }
    }
}