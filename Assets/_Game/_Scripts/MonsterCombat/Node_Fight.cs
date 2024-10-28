using SegoLib.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class Node_Combat : Node
    {
        private class State : NodeState
        {
            public MonsterCombat Combat { get; private set; }

            public float nextHeroDamageTime;

            public State(Node node, IState bb) : base(node)
            {
                Combat = new MonsterCombat(bb.WorldState, bb.Hero, bb.Monster);
            }

            protected override void OnDisposed()
            {
                Combat.Dispose();
                Combat = null;
            }
        }

        [Input] public PortData entry;
        [Output] public PortData victory;
        [Output] public PortData flee;
        [Output] public PortData defeat;

        public override INodeState Enter(IState bb)
        {
            bb.FlavourText = $"Fighting {bb.Monster}.";
            return new State(this, bb);
        }

        public override void OnWorldTimeUpdate(IState bb, float deltaSeconds)
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
                state.Conclude(victory.ConnectedPort.Node);
            } else if (heroDefeated) {
                state.Conclude(defeat.ConnectedPort.Node);
            } else if(ShouldFlee(state, bb.WorldState.Rng)) {
                state.Conclude(flee.ConnectedPort.Node);
            }
        }

        protected override PortData GetPortData(XNode.NodePort port)
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
