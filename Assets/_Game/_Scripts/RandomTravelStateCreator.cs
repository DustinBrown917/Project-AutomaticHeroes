using UnityEngine;

namespace Game
{
    /// <summary>
    /// Wander to a random neighbouring location. More appealing when the hero is rested and it's their preferred time of day,
    /// and less so straight after travelling.
    /// </summary>
    public class RandomTravelStateCreator : HeroTravelStateCreator
    {
        [SerializeField] private Tag m_travelTag;

        public override float Utility(WorldState worldState, Hero hero)
        {
            float wasJustTravellingModifer = hero.Brain.CurrentState.Behaviour.HasTag(m_travelTag) ? 0.5f : 1.0f;

            return (1.0f - hero.Stamina.ExhaustionPercent) * // Exhaustion factor
                ((hero.Core.TimeOfDayPreference.GetPreferenceForCurrentTime() * 0.9f) + 0.1f) * // Time of day factor scaled to 0.1f - 1.0f;
                wasJustTravellingModifer * // To prevent hero from constantly travelling.
                0.5f; // General weight of travel. Maybe replace in the future with something more meaningful?
        }

        protected override bool TryPlanRoute(WorldState worldState, Hero hero, out IRouteTraversal route, out ILocation destination)
        {
            route = GetRandomRoute(worldState, hero.Movement.Location);
            destination = route.End;
            return true;
        }
    }
}
