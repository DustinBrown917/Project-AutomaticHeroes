using Game.Flow;
using SegoLib.Random;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    public class HeroBehaviourNode_Rest : IHeroBehaviour.Node
    {
        protected class State : IHeroBehaviour.INode.State
        {
            public float exitTime;
            public float exhaustionRegenRatePerSecond;
        }

        [Input] public FlowPort entry;
        [Tooltip("How many seconds the rest will run for.")]
        [SerializeField] private float m_restInterval = 3600f;
        [Output] public FlowPort keepResting;
        [Output] public FlowPort doneResting;

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            State state = new();
            state.exitTime = bb.ElapsedTime + m_restInterval;

            if(bb.WorldState.Rng.Bool(GetWakeupChance(bb))) {
                ConcludeViaPort(state, doneResting);
            } else if(bb.TryGetInjectedData(InjectedDataIdentities.RestExhaustionRecoveryRate, out float rate)) {
                state.exhaustionRegenRatePerSecond = rate;
            } else {
                state.exhaustionRegenRatePerSecond = 0.0012f; // About 100/24hrs.
            }

            return state;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(keepResting): return keepResting;
                case nameof(doneResting): return doneResting;
                default: return null;
            }
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds)
        {
            if(node_bb is not State state) {
                ConcludeViaPort(node_bb, doneResting);
                return;
            }

            bb.Hero.Stamina.Exhaustion -= state.exhaustionRegenRatePerSecond;

            if(bb.ElapsedTime >= state.exitTime) {
                ConcludeViaPort(state, keepResting);
            }
        }

        float GetWakeupChance(IHeroBehaviour.IState bb)
        {
            float exhaustionFactor =  1.0f - bb.Hero.Stamina.ExhaustionPercent;
            float timeOfDatFactor = (0.85f * bb.Hero.Core.TimeOfDayPreference.GetPreferenceForCurrentTime()) + 0.1f;

            return exhaustionFactor * timeOfDatFactor;
        }
    }
}
