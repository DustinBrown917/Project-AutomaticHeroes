using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Every location in the world, and route-based navigation between them.
    /// <para>Owns every active <see cref="ILocation"/> among its children. Initialized by <see cref="WorldState"/>.</para>
    /// <para>The navigation graph is rebuilt lazily on the first query after a location is registered, so initializing
    /// before routes have connected themselves (in <c>Route.Awake</c>) is fine.</para>
    /// </summary>
    public class LocationManager : MonoBehaviour, SegoLib.IInitializable
    {
        public IReadOnlyList<ILocation> Locations => m_locations;
        private readonly List<ILocation> m_locations = new();

        private LocationGraph m_locationGraph;

        private bool m_locationGraphNeedsRebuild = true;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            foreach (ILocation location in GetComponentsInChildren<ILocation>()) {
                Register(location);
            }

            IsInitialized = true;
        }

        private void OnDestroy()
        {
            m_locations.Clear();
            m_locationGraph = null;
        }

        /// <summary>
        /// Register <paramref name="location"/> with this manager. Registering the same location twice does nothing.
        /// </summary>
        public void Register(ILocation location)
        {
            if (location == null || m_locations.Contains(location))
                return;

            m_locations.Add(location);

            m_locationGraphNeedsRebuild = true;
        }

        /// <summary>
        /// Find the shortest path from <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="path">Optional. Cleared, then filled with the traversals to take in order.</param>
        /// <returns>False if there's no path.</returns>
        public bool TryFindPath(ILocation start, ILocation end, List<IRouteTraversal> path = null)
        {
            return LocationAStar.TryFindPath(GetGraph(), start, end, path);
        }

        /// <summary>
        /// Find the location closest to <paramref name="start"/> by road that satisfies <paramref name="isGoal"/>. <paramref name="start"/> itself counts.
        /// </summary>
        /// <param name="path">Optional. Cleared, then filled with the traversals to take in order.</param>
        /// <returns>False if no matching location can be reached.</returns>
        public bool TryFindNearest(ILocation start, Predicate<ILocation> isGoal, out ILocation nearest, List<IRouteTraversal> path = null)
        {
            return LocationAStar.TryFindNearest(GetGraph(), start, isGoal, out nearest, path);
        }

        private LocationGraph GetGraph()
        {
            if (m_locationGraphNeedsRebuild) {
                m_locationGraph = new LocationGraph(m_locations);
                m_locationGraphNeedsRebuild = false;
            }

            return m_locationGraph;
        }
    }
}
