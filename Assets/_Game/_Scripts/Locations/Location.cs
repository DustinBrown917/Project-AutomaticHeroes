using Game.Causes;
using Game.UI;
using SegoLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Location : MonoBehaviour, ILocation
    {
        public string DisplayName => name;

        [field: SerializeField] public CauseFavourTracker CauseFavourTracker { get; private set; } = new();
        public Vector2 Position => transform.position;

        public IReadOnlyList<IRoute> Routes => m_routes;

        public IReadOnlyList<Hero> Visitors => m_visitors;
        private readonly List<Hero> m_visitors = new();
        public event ValueUpdatedWithSourceEvent<ILocation, IReadOnlyList<Hero>> visitorsUpdated;

        private readonly List<IRoute> m_routes = new();

        private IHeroBehaviourProvider[] m_heroBehaviourProviders;

        private void Awake()
        {
            CauseFavourTracker.Initialize();
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
        }

        public bool OffersBehaviour(Tag tag)
        {
            if (tag == null)
                return false;

            List<IHeroBehaviourStateCreator> offeredBehaviours = UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Get();

            foreach (IHeroBehaviourProvider stateProvider in m_heroBehaviourProviders) {
                stateProvider.GetStates(offeredBehaviours);
            }

            bool offersBehaviour = false;

            foreach (IHeroBehaviourStateCreator behaviour in offeredBehaviours) {
                if (behaviour.Behaviour != null && behaviour.Behaviour.HasTag(tag)) {
                    offersBehaviour = true;
                    break;
                }
            }

            UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Release(offeredBehaviours);

            return offersBehaviour;
        }

        public void OnLocationEnter(Hero hero)
        {
            if(!m_visitors.Contains(hero)) {
                m_visitors.Add(hero);
                visitorsUpdated?.Invoke(this, Visitors);

                List<IHeroBehaviourStateCreator> behavioursToAdd = UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Get();

                foreach(IHeroBehaviourProvider stateProvider in m_heroBehaviourProviders) {
                    stateProvider.GetStates(behavioursToAdd);
                }

                foreach(IHeroBehaviourStateCreator behaviour in behavioursToAdd) {
                    hero.Brain.AddBehaviourStateCreator(behaviour);
                }

                UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Release(behavioursToAdd);

                CauseFavourTracker.AddFavour(hero.Core.Cause, 5);
            }
        }

        public void OnLocationExit(Hero hero)
        {
            if (m_visitors.Remove(hero)) {
                visitorsUpdated?.Invoke(this, Visitors);

                List<IHeroBehaviourStateCreator> behavioursToRemove = UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Get();

                foreach (IHeroBehaviourProvider stateProvider in m_heroBehaviourProviders) {
                    stateProvider.GetStates(behavioursToRemove);
                }

                foreach (IHeroBehaviourStateCreator behaviour in behavioursToRemove) {
                    hero.Brain.RemoveBehaviourStateCreator(behaviour);
                }

                UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Release(behavioursToRemove);
            }
        }
    }
}
