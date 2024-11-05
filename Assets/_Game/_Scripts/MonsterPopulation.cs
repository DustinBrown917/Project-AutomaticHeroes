using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class MonsterPopulation
    {
        [field: SerializeField] public MonsterDef MonsterDef { get; private set; }
        [field: SerializeField] public float Density { get; private set; }
    }
}
