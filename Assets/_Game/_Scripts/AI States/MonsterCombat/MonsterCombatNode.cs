using Game.Flow;

namespace Game.Monsters.Combat
{
    [NodeWidth(300)]
    public abstract class MonsterCombatNode : FlowNode
    {
        public abstract void Enter(IMonsterCombatState bb);
        public abstract void OnWorldTimeUpdate(IMonsterCombatState bb, float deltaSeconds);
    }
}
