using UnityEngine;

namespace Game
{
    /// <summary>
    /// Sends a wounded hero towards the nearest location (by road) that offers a behaviour tagged as treatment.
    /// <para>Once the hero arrives, that location's own treatment behaviour takes over, and this creator stops qualifying.</para>
    /// </summary>
    public class SeekTreatmentStateCreator : DestinationTravelStateCreator
    {
        [Tooltip("Locations offering a behaviour with this tag can treat wounds.")]
        [SerializeField] private Tag m_treatmentTag;

        [Tooltip("Utility of seeking treatment for a hero on 0 health with nothing treated. It scales down as health or treatment improves.")]
        [SerializeField, Min(0)] private float m_maxUtility = 2.0f;

        /// <summary>
        /// Worth seeking once health is no longer high, and only for wounds that haven't been treated yet.
        /// </summary>
        public override bool CanDo(WorldState worldState, Hero hero)
        {
            return !hero.Heuristics.IsCurrentHealthHigh() &&
                hero.Health.NeedsTreatment() &&
                base.CanDo(worldState, hero);
        }

        public override float Utility(WorldState worldState, Hero hero)
        {
            return (1.0f - hero.Health.Percent) * // Worse wounds are more urgent.
                (1.0f - hero.Health.TreatedPercent) * // Wounds already treated don't need another visit.
                m_maxUtility;
        }

        protected override bool TryGetDestination(WorldState worldState, Hero hero, out ILocation destination)
        {
            return worldState.Locations.TryFindNearest(hero.Movement.Location, OffersTreatment, out destination);
        }

        private bool OffersTreatment(ILocation location) => location.OffersBehaviour(m_treatmentTag);
    }
}
