using Game.Flow;
using SegoLib;
using SegoLib.Random;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    [NodeWidth(300)]
	public class HeroBehaviourNode_Exit : IHeroBehaviour.Node
	{

		/*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
		#region ========================================== Subclasses ==================================================
		/*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/


		protected class State : IHeroBehaviour.INode.State
		{
            public float exitTime;
        }


		/*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
		#endregion ===================================== End Subclasses ================================================
		/*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

		[Input] public FlowPort entry;

        [SerializeField] private FloatRange m_timeToSpendInNode = new FloatRange(3000, 4200);
        [SerializeField] private string[] m_flavours;


		public override FlowPort GetFlowPort(NodePort port)
		{
            switch (port.fieldName) {
                case nameof(entry): return entry;
                default: return null;
            }
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            State state = new State();
            state.exitTime = bb.ElapsedTime + bb.WorldState.Rng.Range(m_timeToSpendInNode);
            bb.FlavourText = bb.WorldState.Rng.GetRandom(m_flavours);
            return state;
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds)
        {
            if (node_bb is not State nodeState)
                return;

            if(bb.ElapsedTime >= nodeState.exitTime) {
                nodeState.Conclude(null);
            }
        }
    } 
}