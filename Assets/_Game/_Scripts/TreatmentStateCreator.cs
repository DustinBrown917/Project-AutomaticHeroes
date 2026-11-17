using SegoLib;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Offered by locations that can treat wounds. A clinic and a hospital are the same component with a different rate.
    /// <para>Its graph should carry the treatment tag, so <see cref="SeekTreatmentStateCreator"/> can find this location.</para>
    /// </summary>
    public class TreatmentStateCreator : HeroBehaviourStateCreator
    {
        [Tooltip("Health treated per hour.")]
        [SerializeField, Min(0)] private float m_treatmentPerHour = 30f;

        [Tooltip("Utility for a hero on 0 health with nothing treated. It scales down as health or treatment improves.")]
        [SerializeField, Min(0)] private float m_maxUtility = 3.0f;

        public override bool CanDo(WorldState worldState, Hero hero)
        {
            return hero.Heuristics.WantsTreatment();
        }

        public override float Utility(WorldState worldState, Hero hero)
        {
            return hero.Heuristics.TreatmentUrgency() * m_maxUtility;
        }

        protected override (Identity key, object value)[] GetInjectedData(WorldState worldState, Hero hero)
        {
            return new (Identity key, object value)[] { (InjectedDataIdentities.TreatmentRate, WorldTime.HoursToSeconds(m_treatmentPerHour)) };
        }
    }
}
