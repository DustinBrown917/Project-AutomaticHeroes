using Game.Flow;
using Game.Monsters.Combat;
using System.Collections.Generic;
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
        [Tooltip("The id of the monster provider to use within the monster provider pool.")]
        [SerializeField] private string m_monsterSource;
        [Tooltip("If no monster provider is found at the above id, or the above id is empty, this will be used as the monster provider instead.")]
        [SerializeField] private Object m_fallbackMonsterProviderObj;

        [System.NonSerialized] private IMonsterProvider m_fallbackMonsterProvider;

        [Input] public FlowPort entry;
        [Output] public FlowPort exit_victory;
        [Output] public FlowPort exit_yield;
        [Output] public FlowPort exit_defeat;

        protected override void Init()
        {
            base.Init();
            m_fallbackMonsterProvider = m_fallbackMonsterProviderObj as IMonsterProvider;
        }

        private void OnValidate()
        {
            if (m_fallbackMonsterProviderObj is not IMonsterProvider)
                m_fallbackMonsterProviderObj = null;
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            State newState = new();

            IMonsterProvider monsterProvider = GetMonsterProvider(bb);

            if (monsterProvider == null) {
                newState.Conclude(exit_yield.ConnectedPortData?.Node as IHeroBehaviour.INode);
                return newState;
            }

            newState.combatState = m_combatGraph.Enter(bb.WorldState, bb.Hero, monsterProvider.GetMonster(bb.WorldState, bb.Hero));

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
                    return exit_victory.ConnectedPortData?.Node;
                case ECombatResult.HeroYield:
                    return exit_yield.ConnectedPortData?.Node;
                case ECombatResult.HeroDowned:
                    return exit_defeat.ConnectedPortData?.Node;
                default:
                    return null;
            }
        }

        protected IMonsterProvider GetMonsterProvider(IHeroBehaviour.IState bb)
        {
            if (string.IsNullOrEmpty(m_monsterSource) || // If source string is null or empty
                !bb.TryGetInjectedData(InjectedDataIdentities.MonsterProviderPool, out MonsterProviderPool pool) || // if no pool was provided
                !pool.TryGetWithFallback(m_monsterSource, out IMonsterProvider provider) // Pool doesn't contain provider with id
                )
                return m_fallbackMonsterProvider;

            return provider;
        }
    }
}
