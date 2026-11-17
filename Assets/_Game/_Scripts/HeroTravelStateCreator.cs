using SegoLib.Random;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Base for behaviours that move a hero along one route to a neighbouring location. Subclasses decide which route,
    /// and how much the hero wants to take it.
    /// <para>
    /// State text is formatted with <c>{0}</c> as the next stop and <c>{1}</c> as the final destination. For a single hop
    /// they're the same location.
    /// </para>
    /// </summary>
    public abstract class HeroTravelStateCreator : HeroBehaviourStateCreator
    {
        public abstract override float Utility(WorldState worldState, Hero hero);

        public override bool CanDo(WorldState worldState, Hero hero)
        {
            return hero.Movement.Location != null &&  // Must be at valid location.
                hero.Movement.Location.Routes.Count > 0 && //Location must have at least one valid route.
                !hero.Heuristics.IsExhaustionHigh(); // Hero must not be exhausted.
        }

        /// <summary>
        /// Choose the route to take from the hero's current location.
        /// </summary>
        /// <param name="route">The route to take now. Must start at the hero's current location.</param>
        /// <param name="destination">Where the hero is ultimately headed, which may be further along than <paramref name="route"/>'s end.</param>
        /// <returns>False if there's no route worth taking.</returns>
        protected abstract bool TryPlanRoute(WorldState worldState, Hero hero, out IRouteTraversal route, out ILocation destination);

        protected override void OnStateCreated(WorldState worldState, Hero hero, IHeroBehaviour.IState createdState)
        {
            if (!TryPlanRoute(worldState, hero, out IRouteTraversal route, out ILocation destination)) {
                // CanDo should rule this out, so the world changed in between. The travel graph departs regardless,
                // and a hero with no route would be left nowhere, so wander instead.
                Debug.LogWarning($"{GetType().Name} couldn't plan a route for {hero.Core.name}. Taking a random route instead.", this);
                route = GetRandomRoute(worldState, hero.Movement.Location);
                destination = route.End;
            }

            hero.Movement.CurrentRoute = route;
            createdState.StateText = string.Format(createdState.StateText, route.End.DisplayName, destination.DisplayName);
        }

        /// <summary>
        /// Pick any route leaving <paramref name="from"/>. <paramref name="from"/> must have at least one route.
        /// </summary>
        protected static IRouteTraversal GetRandomRoute(WorldState worldState, ILocation from)
        {
            IRoute route = worldState.Rng.GetRandom(from.Routes);

            if (route.A == from)
                return new RouteTraversalAToB(route);
            else
                return new RouteTraversalBToA(route);
        }
    }
}
