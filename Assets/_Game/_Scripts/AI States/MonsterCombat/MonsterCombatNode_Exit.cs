using UnityEngine;
using Game.Flow;

namespace Game.Monsters.Combat
{
    [NodeWidth(150)]
    public class MonsterCombatNode_Exit : MonsterCombatNode
    {
        [Input] public FlowPort entry;

        public override void Enter(IMonsterCombatState bb)
        {
            bb.ConcludeCurrentNode(null);
        }

        public override void OnWorldTimeUpdate(IMonsterCombatState bb, float deltaSeconds)
        {

        }

        public override FlowPort GetFlowPort(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                default: return null;
            }
        }
    } 
}
