using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LocationManager : SegoLib.IDisposable
    {
        public IReadOnlyList<ILocation> Locations => m_locations;
        private readonly List<ILocation> m_locations = new();

        private LocationGraph m_locationGraph;

        public bool IsDisposed { get; private set; }

        public event Action<SegoLib.IPrivatelyDisposable> Disposed;

        private bool m_locationGraphNeedsRebuild = true;

        /// <summary>
        /// Ctor.
        /// </summary>
        public LocationManager()
        {

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
        /// Get a path from <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<IRouteTraversal> Navigate(ILocation start, ILocation end)
        {
            if (m_locationGraphNeedsRebuild) {
                m_locationGraph = new LocationGraph(m_locations);
                m_locationGraphNeedsRebuild = false;
            }

            return LocationAStar.FindPath(m_locationGraph, start, end);
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            m_locations.Clear();
            m_locationGraph = null;
            Disposed?.Invoke(this);
            Disposed = null;
        }
    }
}
