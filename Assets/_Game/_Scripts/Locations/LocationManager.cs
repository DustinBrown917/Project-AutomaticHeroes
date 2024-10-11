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

        public event Action<SegoLib.IDisposable> disposed;

        private bool m_locationGraphNeedsRebuild = true;

        /// <summary>
        /// Ctor.
        /// </summary>
        public LocationManager()
        {

        }

        /// <summary>
        /// Register <paramref name="location"/> with this manager. <paramref name="locationId"/> should be a reference to the underlying value of <paramref name="location"/>.Id.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="locationId"></param>
        public void Register(ILocation location, out int locationId)
        {
            locationId = m_locations.Count;

            if (location.Id != locationId) {
                Debug.LogError($"Error registering location {location.DisplayName}. Argument '{nameof(locationId)}' should be a reference to the underlying value of '{nameof(location)}.{nameof(ILocation.Id)}'.");
                return;
            }

            m_locations.Add(location);

            m_locationGraphNeedsRebuild = true;
        }

        public void CopyTo(LocationManager other)
        {
            other.m_locations.Clear();

            foreach (ILocation location in other.m_locations) {
                m_locations.Add(location);
            }
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
            IsDisposed = true;
            m_locations.Clear();
            disposed?.Invoke(this);
        }
    }
}
