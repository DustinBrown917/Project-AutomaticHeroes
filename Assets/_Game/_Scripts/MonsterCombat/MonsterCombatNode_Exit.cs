using UnityEngine;
using Game.Flow;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Exit : MonsterCombatNode
    {
        [Input] public FlowPort entry;

        public override IFlowNodeState Enter(IFlowState bb)
        {
            FlowNodeState newState = new FlowNodeState(this);
            newState.Conclude(null);
            return newState;
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            
        }

        public override FlowPort GetPortData(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                default: return null;
            }
        }
    } 
}
