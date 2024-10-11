using SegoLib;
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

        public IReadOnlyList<Hero> Visitors => m_visitors;
        private readonly List<Hero> m_visitors = new();
        public event ValueUpdatedWithSourceEvent<ILocation, IReadOnlyList<Hero>> visitorsUpdated;

        private readonly List<IRoute> m_routes = new();

        private TravelToRandomNeighbour m_travelToRandomNeighbourFactory = new TravelToRandomNeighbour();

        private IHeroStateProvider[] m_heroStateProviders;

        private void Awake()
        {
            m_heroStateProviders = GetComponentsInChildren<IHeroStateProvider>(true);
        }

        public void AddRoute(IRoute route)
        {
            m_routes.Add(route);
        }

        public void OnLocationEnter(Hero hero)
        {
            if(!m_visitors.Contains(hero)) {
                m_visitors.Add(hero);
                visitorsUpdated?.Invoke(this, Visitors);

                hero.Brain.AddStateFactory(m_travelToRandomNeighbourFactory);

                List<IStateFactory> states = UnityEngine.Pool.ListPool<IStateFactory>.Get();
                foreach(IHeroStateProvider stateProvider in m_heroStateProviders) {
                    stateProvider.GetStates(states);
                }

                foreach(IStateFactory stateFactory in states) {
                    hero.Brain.AddStateFactory(stateFactory);
                }

                UnityEngine.Pool.ListPool<IStateFactory>.Release(states);
            }
        }

        public void OnLocationExit(Hero hero)
        {
            if (m_visitors.Remove(hero)) {
                visitorsUpdated?.Invoke(this, Visitors);

                hero.Brain.RemoveStateFactory(m_travelToRandomNeighbourFactory);

                List<IStateFactory> states = UnityEngine.Pool.ListPool<IStateFactory>.Get();
                foreach (IHeroStateProvider stateProvider in m_heroStateProviders) {
                    stateProvider.GetStates(states);
                }

                foreach (IStateFactory stateFactory in states) {
                    hero.Brain.RemoveStateFactory(stateFactory);
                }

                UnityEngine.Pool.ListPool<IStateFactory>.Release(states);
            }
        }
    }
}
