using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class MonsterPopulation
    {
        [field: SerializeField] public MonsterDef MonsterDef { get; private set; }
        [field: SerializeField] public float Density { get; private set; }

        public MonsterPopulation(MonsterDef monsterDef, float density)
        {
            MonsterDef = monsterDef;
            Density = density;
        }
    }
}
