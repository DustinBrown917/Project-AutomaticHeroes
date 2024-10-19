using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Monster Def", menuName = "Game/MonsterDef")]
    public class MonsterDef : ScriptableObject
    {
        [field: SerializeField] public float HealthPerLevel { get; private set; }
        [field: SerializeField] public float ExpPerLevel { get; private set; }
        [field: SerializeField] public float DPS { get; private set; }
    } 

    public class Monster
    {
        public MonsterDef Def { get; }
        public int Level { get; }
        public float DPS => Def.DPS * Level;
        public int ExpReward { get; }

        public float MaxHealth { get; }
        public float CurrentHealth { get; set; }

        public Monster(MonsterDef def, int level)
        {
            Def = def;
            Level = level;
            ExpReward = Mathf.RoundToInt(def.ExpPerLevel * level);

            MaxHealth = CurrentHealth = Mathf.Max(1, Def.HealthPerLevel * level);
        }
    }
}
