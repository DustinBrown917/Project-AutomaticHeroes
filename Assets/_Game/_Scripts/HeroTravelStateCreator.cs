using SegoLib.Random;
using UnityEngine;

namespace Game
{
    public class HeroTravelStateCreator : HeroBehaviourStateCreator
    {
        [SerializeField] private Tag m_travelTag;

        private IRouteTraversal GetRoute(WorldState worldState, Hero hero)
        {
            ILocation location = hero.Movement.Location;

            IRoute route = worldState.Rng.GetRandom(location.Routes);

            IRouteTraversal routeTraversal = null;

            if (route.A == location)
                routeTraversal = new RouteTraversalAToB(route);
            else
                routeTraversal = new RouteTraversalBToA(route);


            return routeTraversal;
        }

        public override float Utility(WorldState worldState, Hero hero)
        {
            float wasJustTravellingModifer = hero.Brain.CurrentState.Behaviour.HasTag(m_travelTag) ? 0.5f : 1.0f;

            return (1.0f - hero.Stamina.ExhaustionPercent) * // Exhaustion factor
                ((hero.Core.TimeOfDayPreference.GetPreferenceForCurrentTime() * 0.9f) + 0.1f) * // Time of day factor scaled to 0.1f - 1.0f;
                wasJustTravellingModifer * // To prevent hero from constantly travelling.
                0.5f; // General weight of travel. Maybe replace in the future with something more meaningful?
        }

        public override bool CanDo(WorldState worldState, Hero hero)
        {
            return hero.Movement.Location != null &&  // Must be at valid location.
                hero.Movement.Location.Routes.Count > 0 && //Location must have at least one valid route.
                !hero.Heuristics.IsExhaustionHigh(); // Hero must not be exhausted.
        }

        protected override void OnStateCreated(WorldState worldState, Hero hero, IHeroBehaviour.IState createdState)
        {
            hero.Movement.CurrentRoute = GetRoute(worldState, hero);
            createdState.StateText = string.Format(createdState.StateText, hero.Movement.CurrentRoute.End.DisplayName);
        }
    } 
}
