using SegoLib.Random;
using Game.Flow;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Combat : MonsterCombatNode
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        private class State : FlowNodeState
        {
            public MonsterCombat Combat { get; private set; }
            public float timeInCombat;

            public State(MonsterCombatNode node, IMonsterCombatState bb) : base(node)
            {
                Combat = new MonsterCombat(bb.WorldState, bb.Hero, bb.Monster);
            }

            protected override void OnDisposed()
            {
                Combat.Dispose();
                Combat = null;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        [Input] public FlowPort entry;
        [Output] public FlowPort victory;
        [Output] public FlowPort flee;
        [Output] public FlowPort defeat;

        public override IFlowNodeState Enter(IFlowState bb)
        {
            if (bb is not IMonsterCombatState state)
                throw new System.ArgumentException($"Must be of type {nameof(IMonsterCombatState)}.", nameof(bb));

            bb.FlavourText = $"Fighting {state.Monster}.";
            return new State(this, state);
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            state.timeInCombat += deltaSeconds;
            state.Combat.Simulate(deltaSeconds);

            bool monsterDefeated = state.Combat.Monster.CurrentHealth <= 0;
            bool heroDefeated = state.Combat.Hero.Health.Current<= 0;

            if (monsterDefeated) {
                state.Conclude(victory.ConnectedPortData.Node);
            } else if (heroDefeated) {
                state.Conclude(defeat.ConnectedPortData.Node);
            } else if(state.Combat.HeroIsAttemptingToFlee) {
                state.Conclude(flee.ConnectedPortData.Node);
            }
        }

        public override FlowPort GetFlowPort(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(defeat): return defeat;
                case nameof(flee): return flee;
                case nameof(victory): return victory;
                default: return null;
            }
        }
    }
}
