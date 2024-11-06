using Game.Flow;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    [NodeWidth(150)]
	public class HeroBehaviourNode_Exit : IHeroBehaviour.Node
	{

		[Input] public FlowPort entry;


		public override FlowPort GetFlowPort(NodePort port)
		{
            switch (port.fieldName) {
                case nameof(entry): return entry;
                default: return null;
            }
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State state = new();
            state.Conclude(null);
            return state;
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds)
        {

        }
    } 
}