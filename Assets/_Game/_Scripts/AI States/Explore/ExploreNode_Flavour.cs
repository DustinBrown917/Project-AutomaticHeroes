using Game.Flow;
using SegoLib;
using SegoLib.Random;
using UnityEngine;
using XNode;

namespace Game.Exploration
{
    [NodeWidth(300)]
	public class ExploreNode_Flavour : ExploreNode
	{

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : FlowNodeState
        {
            public float exitTime;

            public State(ExploreNode node) : base(node) { }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        [Input] public FlowPort entry;

        [SerializeField] private FloatRange m_timeToSpendInNode = new FloatRange(3000, 4200);
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

        public override IFlowNodeState Enter(IFlowState bb)
        {
            State state = new State(this);
            state.exitTime = bb.ElapsedTime + bb.WorldState.Rng.Range(m_timeToSpendInNode);
            bb.FlavourText = bb.WorldState.Rng.GetRandom(m_flavours);
            return state;
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if(bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.exitTime) {
                state.Conclude(exit.ConnectedPortData.Node);
            }
        }
    } 
}