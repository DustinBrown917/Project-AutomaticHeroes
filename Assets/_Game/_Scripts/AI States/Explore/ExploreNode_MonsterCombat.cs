using Game.Flow;
using Game.Monsters.Combat;
using SegoLib.Random;
using UnityEngine;
using XNode;

namespace Game.Exploration
{
    [NodeWidth(300)]
    public class ExploreNode_MonsterCombat : ExploreNode
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : FlowNodeState
        {
            public IMonsterCombatState combatState;

            public State(ExploreNode node) : base(node) { }

            protected override void OnDisposed()
            {
                combatState?.Dispose();
                combatState = null;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private MonsterHunt.MonsterPopulation[] m_monsters;
        [SerializeField] private MonsterCombatGraph m_combatGraph;

        [System.NonSerialized] private float m_sumOfMonsterPopulation;

        [Input] public FlowPort entry;
        [Output] public FlowPort exit_victory;
        [Output] public FlowPort exit_flee;
        [Output] public FlowPort exit_defeat;

        protected override void Init()
        {
            base.Init();
            m_sumOfMonsterPopulation = 0;
            if(m_monsters != null) {
                foreach (MonsterHunt.MonsterPopulation monster in m_monsters) {
                    m_sumOfMonsterPopulation += monster.Density;
                }
            }
        }

        public override IFlowNodeState Enter(IFlowState bb)
        {
            State newState = new State(this);

            newState.combatState = m_combatGraph.Enter(bb.WorldState, bb.Hero, GetMonster(bb));

            bb.FlavourText = newState.combatState.FlavourText;

            // Update the flavour text of bb when combat state flavour updates.
            // This is cleaned up automatically when the nodestate is disposed.
            newState.combatState.flavourTextUpdated += (flowState, text) => bb.FlavourText = text;

            return newState;
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            state.combatState.OnWorldTimeUpdate(deltaSeconds);

            if (state.combatState.Status == IFlowState.EStatus.Concluded) {
                state.Conclude(GetNextNode(state));
            }
        }

        public override FlowPort GetFlowPort(NodePort nodePort)
        {
            switch (nodePort.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit_victory): return exit_victory;
                case nameof(exit_flee): return exit_flee;
                case nameof(exit_defeat): return exit_defeat;
                default: return null;
            }
        }

        private Monster GetMonster(IFlowState bb)
        {
            int index = WeightedRandom.Naive(m_monsters, m_sumOfMonsterPopulation, bb.WorldState.Rng, x => x.Density);
            return new Monster(m_monsters[index].MonsterDef);
        }

        private FlowNode GetNextNode(State state)
        {
            switch (state.combatState.Result) {
                case IMonsterCombatState.ECombatResult.Victorious:
                    return exit_victory.ConnectedPortData.Node;
                case IMonsterCombatState.ECombatResult.Fled:
                    return exit_flee.ConnectedPortData.Node;
                case IMonsterCombatState.ECombatResult.Defeated:
                    return exit_defeat.ConnectedPortData.Node;
                default:
                    return null;
            }
        }

        private void ApplyExperience()
        {

        }
    } 
}
