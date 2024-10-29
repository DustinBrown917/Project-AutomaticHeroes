using SegoLib.Random;
using Game.Flow;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Combat : MonsterCombatNode
    {
        private class State : FlowNodeState
        {
            public MonsterCombat Combat { get; private set; }

            public float nextHeroDamageTime;

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

            state.Combat.SimulateStepAndApply(deltaSeconds);

            bool monsterDefeated = state.Combat.Monster.CurrentHealth <= 0;
            bool heroDefeated = state.Combat.Hero.Health.Current - state.Combat.accumulatedHeroDamage <= 0;

            // Do damage update for the hero's UI
            if (bb.ElapsedTime >= state.nextHeroDamageTime || heroDefeated || monsterDefeated) {
                state.Combat.Hero.Health.Damage(state.Combat.accumulatedHeroDamage);
                state.Combat.accumulatedHeroDamage = 0;
                state.nextHeroDamageTime = bb.ElapsedTime + GetNextHeroDamageTime(bb.WorldState.Rng);
            }

            if (monsterDefeated) {
                state.Conclude(victory.ConnectedPortData.Node);
            } else if (heroDefeated) {
                state.Conclude(defeat.ConnectedPortData.Node);
            } else if(ShouldFlee(state, bb.WorldState.Rng)) {
                state.Conclude(flee.ConnectedPortData.Node);
            }
        }

        public override FlowPort GetPortData(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(defeat): return defeat;
                case nameof(flee): return flee;
                case nameof(victory): return victory;
                default: return null;
            }
        }

        private float GetNextHeroDamageTime(IRandomNumberGenerator rng)
        {
            return rng.Range(300, 1600);
        }

        private bool ShouldFlee(State state, IRandomNumberGenerator rng)
        {
            float timeToHeroVictory = state.Combat.GetTimeToHeroVictoryInSeconds();
            float timeToHeroDefeat = state.Combat.GetTimeToMonsterVictoryInSeconds();

            if (timeToHeroVictory < timeToHeroDefeat)
                return false;

            return rng.Bool(1.0f - (timeToHeroVictory / timeToHeroDefeat));
        }
    }
}
