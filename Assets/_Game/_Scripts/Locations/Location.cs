using Game.UI;
using SegoLib;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Location : MonoBehaviour, ILocation
    {
        public string DisplayName => name;

        public int Id { get; private set; }

        public Vector2 Position => transform.position;

        public IReadOnlyList<IRoute> Routes => m_routes;

        public IReadOnlyList<Hero> Visitors => m_visitors;
        private readonly List<Hero> m_visitors = new();
        public event ValueUpdatedWithSourceEvent<ILocation, IReadOnlyList<Hero>> visitorsUpdated;

        private readonly List<IRoute> m_routes = new();
        private readonly List<Travel> m_travelStates = new();

        private IHeroBehaviourProvider[] m_heroBehaviourProviders;

        private void Awake()
        {
            m_heroBehaviourProviders = GetComponentsInChildren<IHeroBehaviourProvider>(true);
        }

        private void Start()
        {
            VM_ILocation vm = GetComponent<VM_ILocation>();
            vm.SetModel(this);
        }

        public void AddRoute(IRoute route)
        {
            m_routes.Add(route);
            if(route.A == (ILocation)this) {
                m_travelStates.Add(new Travel(new RouteTraversalAToB(route)));
            } else {
                m_travelStates.Add(new Travel(new RouteTraversalBToA(route)));
            }
        }

        public void OnLocationEnter(Hero hero)
        {
            if(!m_visitors.Contains(hero)) {
                m_visitors.Add(hero);
                visitorsUpdated?.Invoke(this, Visitors);

                List<IHeroBehaviour> behaviours = UnityEngine.Pool.ListPool<IHeroBehaviour>.Get();

                behaviours.AddRange(m_travelStates);

                foreach(IHeroBehaviourProvider stateProvider in m_heroBehaviourProviders) {
                    stateProvider.GetStates(behaviours);
                }

                foreach(IHeroBehaviour behaviour in behaviours) {
                    hero.Brain.AddBehaviour(behaviour);
                }

                UnityEngine.Pool.ListPool<IHeroBehaviour>.Release(behaviours);
            }
        }

        public void OnLocationExit(Hero hero)
        {
            if (m_visitors.Remove(hero)) {
                visitorsUpdated?.Invoke(this, Visitors);

                List<IHeroBehaviour> behaviours = UnityEngine.Pool.ListPool<IHeroBehaviour>.Get();

                behaviours.AddRange(m_travelStates);

                foreach (IHeroBehaviourProvider stateProvider in m_heroBehaviourProviders) {
                    stateProvider.GetStates(behaviours);
                }

                foreach (IHeroBehaviour behaviour in behaviours) {
                    hero.Brain.RemoveBehaviour(behaviour);
                }

                UnityEngine.Pool.ListPool<IHeroBehaviour>.Release(behaviours);
            }
        }
    }
}
