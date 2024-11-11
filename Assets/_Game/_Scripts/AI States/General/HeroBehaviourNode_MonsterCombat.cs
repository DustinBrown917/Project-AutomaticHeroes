using Game.Flow;
using Game.Monsters.Combat;
using SegoLib.Random;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    [NodeWidth(300)]
    public class HeroBehaviourNode_MonsterCombat : IHeroBehaviour.Node
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.INode.State
        {
            public IMonsterCombatState combatState;

            protected override void OnDisposed()
            {
                combatState?.Dispose();
                combatState = null;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private MonsterCombatGraph m_combatGraph;
        [SerializeField] private Object m_monsterProviderObject;

        [System.NonSerialized] private IMonsterProvider m_monsterProvider;

        [Input] public FlowPort entry;
        [Output] public FlowPort exit_victory;
        [Output] public FlowPort exit_yield;
        [Output] public FlowPort exit_defeat;

        protected override void Init()
        {
            base.Init();
            m_monsterProvider = m_monsterProviderObject as IMonsterProvider;
        }

        private void OnValidate()
        {
            if (m_monsterProviderObject is not IMonsterProvider)
                m_monsterProviderObject = null;
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            State newState = new();

            if(m_monsterProvider == null) {
                newState.Conclude(exit_yield.ConnectedPortData.Node as IHeroBehaviour.INode);
                return newState;
            }

            newState.combatState = m_combatGraph.Enter(bb.WorldState, bb.Hero, m_monsterProvider.GetMonster(bb.WorldState, bb.Hero));

            bb.FlavourText = newState.combatState.FlavourText;

            // Update the flavour text of bb when combat state flavour updates.
            // This is cleaned up automatically when the nodestate is disposed.
            newState.combatState.FlavourTextUpdated += (flowState, text) => bb.FlavourText = text;

            return newState;
        }


        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds)
        {
            if (node_bb is not State state)
                return;

            state.combatState.OnWorldTimeUpdate(deltaSeconds);

            if (state.combatState.CombatIsConcluded) {
                bb.InjectData(InjectedDataIdentities.PreviousCombatSummary, state.combatState.GetSummary());
                state.Conclude(GetNextNode(state) as IHeroBehaviour.INode);
            }
        }

        public override FlowPort GetFlowPort(NodePort nodePort)
        {
            switch (nodePort.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit_victory): return exit_victory;
                case nameof(exit_yield): return exit_yield;
                case nameof(exit_defeat): return exit_defeat;
                default: return null;
            }
        }

        private FlowNode GetNextNode(State state)
        {
            switch (state.combatState.Result) {
                case ECombatResult.Victorious:
                    return exit_victory.ConnectedPortData.Node;
                case ECombatResult.HeroYield:
                    return exit_yield.ConnectedPortData.Node;
                case ECombatResult.HeroDowned:
                    return exit_defeat.ConnectedPortData.Node;
                default:
                    return null;
            }
        }
    } 

}
