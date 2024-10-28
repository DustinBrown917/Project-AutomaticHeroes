

namespace Game.Monsters.Combat
{
    public class Node_Enter : Node
    {
        public class State : NodeState
        {
            public State(Node node) : base(node)
            {
            }
        }

        [Output] public PortData exit;

        public override INodeState Enter(IState bb)
        {
            State state = new State(this);

            state.Conclude(exit.ConnectedPort.Node);

            return state;
        }

        public override void OnWorldTimeUpdate(IState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;
        }

        protected override PortData GetPortData(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(exit): return exit;
                default: return null;
            }
        }
    } 
}
