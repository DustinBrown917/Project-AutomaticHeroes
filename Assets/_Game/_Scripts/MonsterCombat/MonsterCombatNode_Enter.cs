using Game.Flow;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Enter : MonsterCombatNode
    {
        public class State : FlowNodeState
        {
            public State(MonsterCombatNode node) : base(node)
            {
            }
        }

        [Output] public FlowPort exit;

        public override IFlowNodeState Enter(IFlowState bb)
        {
            State state = new State(this);

            state.Conclude(exit.ConnectedPortData.Node);

            return state;
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;
        }

        public override FlowPort GetPortData(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(exit): return exit;
                default: return null;
            }
        }
    } 
}
