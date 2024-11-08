using Game.Flow;
using SegoLib.Random;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    [NodeWidth(300)]
    public class HeroBehaviourNode_Flavour : IHeroBehaviour.Node
    {

        [Input] public FlowPort entry;

        [SerializeField] private string[] m_flavours;

        [Output] public FlowPort exit;

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit): return exit;
                default: return null;
            }
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State state = new();
            bb.FlavourText = bb.Format(bb.WorldState.Rng.GetRandom(m_flavours));
            state.Conclude(exit.ConnectedPortData.Node as IHeroBehaviour.INode);
            return state;
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds) { }
    }
}
