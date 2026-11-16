using Game.Flow;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    public class HeroBehaviourNode_ElapsedTimeCheck : IHeroBehaviour.Node
    {
        public enum EComparison : byte
        {
            EqualTo,
            NotEqualTo,
            GreaterThan,
            LessThan,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }

        [Input] public FlowPort entry;
        [SerializeField] private EComparison m_comparison;
        [SerializeField] private float m_comparisonTime = 3600;
        [Output] public FlowPort conditionTrue;
        [Output] public FlowPort conditionFalse;

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State state = new();

            if (DoComparison(bb)) {
                state.Conclude(conditionTrue.ConnectedPortData?.Node as IHeroBehaviour.INode);
            } else {
                state.Conclude(conditionFalse.ConnectedPortData?.Node as IHeroBehaviour.INode);
            }

            return state;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(conditionTrue): return conditionTrue;
                case nameof(conditionFalse): return conditionFalse;
                default: return null;
            }
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds) { }

        private bool DoComparison(IHeroBehaviour.IState bb)
        {
            switch (m_comparison) {
                case EComparison.EqualTo: return m_comparisonTime == bb.ElapsedTime;
                case EComparison.NotEqualTo: return m_comparisonTime != bb.ElapsedTime;
                case EComparison.GreaterThan: return m_comparisonTime < bb.ElapsedTime;
                case EComparison.LessThan: return m_comparisonTime > bb.ElapsedTime;
                case EComparison.GreaterThanOrEqualTo: return m_comparisonTime <= bb.ElapsedTime;
                case EComparison.LessThanOrEqualTo: return m_comparisonTime >= bb.ElapsedTime;
                default: return true;
            }
        }
    }

}