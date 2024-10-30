using SegoLib.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Game.Flow
{
    public class FlowNode_RandomBranch : FlowNode
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        public class WeightedFlowPort : FlowPort
        {
            public float weight = 1.0f;
        }


        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        [Input] public FlowPort entry;
        [Output(dynamicPortList = true)] public WeightedFlowPort[] exits;

        [System.NonSerialized] private float m_sumOfWeights;

        protected override void Init()
        {
            base.Init();
            if(exits != null) {
                foreach(WeightedFlowPort port in exits) {
                    m_sumOfWeights += port.weight;
                }
            }
        }

        public override IFlowNodeState Enter(IFlowState bb)
        {
            FlowNodeState state = new FlowNodeState(this);
            state.Conclude(GetRandomNode(bb.WorldState.Rng));
            return state;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            if(port.fieldName == nameof(entry))
                return entry;

            if (port.fieldName == nameof(exits) || !port.fieldName.Contains(nameof(exits)))
                return null;

            int index = int.Parse(port.fieldName.Substring(nameof(exits).Length));
            return exits[index];
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            
        }

        private FlowNode GetRandomNode(IRandomNumberGenerator rng)
        {
            if (m_sumOfWeights <= 0)
                return rng.GetRandom(exits).ConnectedPortData.Node;

            int index = WeightedRandom.Naive(exits, m_sumOfWeights, rng, x => x.weight);
            return exits[index].ConnectedPortData.Node;
        }
    } 
}
