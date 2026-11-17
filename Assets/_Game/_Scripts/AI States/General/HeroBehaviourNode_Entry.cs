using Game.Flow;
using XNode;

namespace Game.AI.General
{
    [NodeWidth(150)]
    public class HeroBehaviourNode_Entry : IHeroBehaviour.Node
    {

        [Output] public FlowPort exit;

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State newState = new IHeroBehaviour.INode.State();
            newState.Conclude(exit.ConnectedPortData?.Node as IHeroBehaviour.INode);

            return newState;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(exit): return exit;
                default: return null;
            }
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds) { }
    }
}
