using Game.Flow;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    /// <summary>
    /// Treats the hero's wounds over time. Treated health doesn't heal on its own, it speeds up the hero's natural recovery
    /// (see <see cref="HeroHealth.Recover"/>), so the benefit carries on after the hero leaves.
    /// <para>
    /// The rate comes from <see cref="InjectedDataIdentities.TreatmentRate"/> (per second) if the behaviour provides one,
    /// so the same graph can serve a village clinic and a city hospital.
    /// </para>
    /// </summary>
    public class HeroBehaviourNode_TreatWounds : IHeroBehaviour.Node
    {
        protected class State : IHeroBehaviour.INode.State
        {
            public float exitTime;
            public float treatmentPerSecond;
        }

        [Input] public FlowPort entry;
        [Tooltip("How many seconds of treatment before leaving through Keep Treating, so flavour can be interleaved.")]
        [SerializeField] private float m_treatmentInterval = 3600f;
        [Tooltip("Health treated per hour when the behaviour doesn't provide a rate.")]
        [SerializeField, Min(0)] private float m_fallbackTreatmentPerHour = 20f;
        [Output] public FlowPort keepTreating;
        [Output] public FlowPort doneTreating;

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            State state = new();
            state.exitTime = bb.ElapsedTime + m_treatmentInterval;

            if (bb.TryGetInjectedData(InjectedDataIdentities.TreatmentRate, out float rate)) {
                state.treatmentPerSecond = rate;
            } else {
                state.treatmentPerSecond = WorldTime.HoursToSeconds(m_fallbackTreatmentPerHour);
            }

            if (!bb.Hero.Health.NeedsTreatment()) {
                ConcludeViaPort(state, doneTreating);
            }

            return state;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(keepTreating): return keepTreating;
                case nameof(doneTreating): return doneTreating;
                default: return null;
            }
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds)
        {
            if (node_bb is not State state) {
                ConcludeViaPort(node_bb, doneTreating);
                return;
            }

            bb.Hero.Health.Treated += state.treatmentPerSecond * deltaSeconds;

            if (!bb.Hero.Health.NeedsTreatment()) {
                ConcludeViaPort(state, doneTreating);
            } else if (bb.ElapsedTime >= state.exitTime) {
                ConcludeViaPort(state, keepTreating);
            }
        }
    }
}
