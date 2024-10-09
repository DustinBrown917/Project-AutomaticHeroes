using SegoLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Location : MonoBehaviour, ILocation
    {
        public string DisplayName => name;

        public int Id => m_id;
        private int m_id;

        public Vector2 Position => transform.position;

        public IReadOnlyList<IRoute> Routes => m_routes;
        private readonly List<IRoute> m_routes = new();


        private void Start()
        {
        }

        public void AddRoute(IRoute route)
        {
            m_routes.Add(route);
        }
    }
}
