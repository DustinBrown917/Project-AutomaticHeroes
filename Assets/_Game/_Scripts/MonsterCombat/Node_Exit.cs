using UnityEngine;

namespace Game.Monsters.Combat
{
    public class Node_Exit : Node
    {
        [Input] public PortData entry;

        public override INodeState Enter(IState bb)
        {
            NodeState newState = new NodeState(this);
            newState.Conclude(null);
            return newState;
        }

        public override void OnWorldTimeUpdate(IState bb, float deltaSeconds)
        {
            
        }

        protected override PortData GetPortData(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                default: return null;
            }
        }
    } 
}
