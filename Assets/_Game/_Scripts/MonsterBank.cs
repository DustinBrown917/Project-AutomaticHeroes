using UnityEngine;

namespace Game
{

    public interface IMonsterProvider
    {
        public Monster GetMonster(WorldState worldState, Hero hero);
    }

    public abstract class MonsterBank : ScriptableObject, IMonsterProvider
    {
        public abstract Monster GetMonster(WorldState worldState, Hero hero);
    }
}