using Game.Flow;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    public class HeroBehaviourNode_Travel : IHeroBehaviour.Node
    {
        [Input] public FlowPort entry;
        [Output] public FlowPort exit;

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State state = new();
            return state;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit): return exit;
                default: return null;
            }
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds)
        {
            bb.Hero.Stamina.TrySpendEnergy(HeroStamina.MOVEMENT_STAMINA_PER_SECOND * deltaSeconds);
            bb.Hero.Movement.ProgressBySeconds(deltaSeconds);

            if(bb.Hero.Movement.RouteProgress >= 1.0f) {
                node_bb.Conclude(exit.ConnectedPortData?.Node as IHeroBehaviour.INode);
            }
        }
    }
}
