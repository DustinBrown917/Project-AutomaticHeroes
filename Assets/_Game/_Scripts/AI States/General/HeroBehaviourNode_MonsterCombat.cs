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

        [SerializeField] private MonsterPopulation[] m_monsters;
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
                foreach (MonsterPopulation monster in m_monsters) {
                    m_sumOfMonsterPopulation += monster.Density;
                }
            }
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            State newState = new();

            newState.combatState = m_combatGraph.Enter(bb.WorldState, bb.Hero, GetMonster(bb));

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
                state.Conclude(GetNextNode(state) as IHeroBehaviour.INode);
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

        private Monster GetMonster(IHeroBehaviour.IState bb)
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
    } 
}
