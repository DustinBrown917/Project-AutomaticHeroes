using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// The hero takes the first route on the shortest path to a destination chosen by the subclass.
    /// <para>
    /// Only one hop is planned at a time. When the hero arrives, the brain chooses again, so a long journey is a series of
    /// decisions: the hero can stop to rest on the way, get distracted, or change its mind if the destination stops mattering.
    /// </para>
    /// </summary>
    public abstract class DestinationTravelStateCreator : HeroTravelStateCreator
    {
        /// <summary>
        /// Where the hero wants to go, if anywhere. Returning the hero's current location means it has already arrived.
        /// </summary>
        protected abstract bool TryGetDestination(WorldState worldState, Hero hero, out ILocation destination);

        public override bool CanDo(WorldState worldState, Hero hero)
        {
            return base.CanDo(worldState, hero) && TryPlanRoute(worldState, hero, out _, out _);
        }

        /// <returns>False if there is no destination, the hero is already there, or it can't be reached.</returns>
        protected override bool TryPlanRoute(WorldState worldState, Hero hero, out IRouteTraversal route, out ILocation destination)
        {
            route = null;

            if (!TryGetDestination(worldState, hero, out destination))
                return false;

            List<IRouteTraversal> path = UnityEngine.Pool.ListPool<IRouteTraversal>.Get();

            if (worldState.Locations.TryFindPath(hero.Movement.Location, destination, path) && path.Count > 0)
                route = path[0];

            UnityEngine.Pool.ListPool<IRouteTraversal>.Release(path);

            return route != null;
        }
    }
}
