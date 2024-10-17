using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Monster Def", menuName = "Game/MonsterDef")]
    public class MonsterDef : ScriptableObject
    {
        [field: SerializeField] public float ExpPerLevel { get; private set; }
        [field: SerializeField] public float AttackDamage { get; private set; }
    } 

    public class Monster
    {
        public MonsterDef Def { get; }
        public int Level { get; }
        public float AttackDamage => Def.AttackDamage * Level;
        public int ExpReward { get; }

        public Monster(MonsterDef def, int level)
        {
            Def = def;
            Level = level;
            ExpReward = Mathf.RoundToInt(def.ExpPerLevel * level);
        }
    }
}
