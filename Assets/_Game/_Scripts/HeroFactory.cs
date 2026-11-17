using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroFactory : MonoBehaviour
    {
        public delegate void HeroCreatedEvent(HeroFactory sender, Hero hero);
        public delegate void HeroRetiredEvent(HeroFactory sender, Hero hero);

        [SerializeField] private Hero m_heroPrefab;
        
        private readonly Dictionary<HeroCore, Hero> m_createdHeroes = new();

        private int m_nextHeroId;

        private WorldState m_worldState;

        public bool IsInitialized { get; private set; }

        public event HeroCreatedEvent HeroCreated;
        public event HeroRetiredEvent HeroRetired;

        /// <summary>
        /// Give the factory the world that every hero it creates will live in. Does nothing if already initialized.
        /// </summary>
        public void Initialize(WorldState worldState)
        {
            if (IsInitialized)
                return;

            if (worldState == null) {
                Debug.LogError($"{nameof(HeroFactory)} must be initialized with a valid {nameof(WorldState)}.", this);
                return;
            }

            m_worldState = worldState;
            IsInitialized = true;
        }

        private void OnDestroy()
        {
            foreach(Hero hero in m_createdHeroes.Values) {
                if (hero != null)
                    hero.Destroyed -= RetireHero;
            }

            m_createdHeroes.Clear();
            m_heroPrefab = null;
            m_worldState = null;
        }

        /// <summary>
        /// Remove a hero from this hero factory and free their core for use.
        /// </summary>
        private void RetireHero(Hero hero)
        {
            if (hero == null || hero.Core == null) {
                return;
            }

            m_createdHeroes.Remove(hero.Core);
            hero.Destroyed -= RetireHero;
            HeroRetired?.Invoke(this, hero);
        }

        public bool TryCreateHero(HeroCore core, ILocation location, out Hero hero)
        {
            if (!IsInitialized) {
                Debug.LogError($"{nameof(HeroFactory)} must be initialized before creating heroes.", this);
                hero = null;
                return false;
            }

            if (core == null) {
                Debug.LogError($"A valid {nameof(HeroCore)} must be provided.", this);
                hero = null;
                return false;
            }

            if (m_createdHeroes.ContainsKey(core)) {
                Debug.LogError($"A hero has already been created for the provided {nameof(HeroCore)}: {core.name}.", this);
                hero = null;
                return false;
            }

            if (location == null) {
                Debug.LogError($"A valid {nameof(ILocation)} must be provided.", this);
                hero = null;
                return false;
            }

            hero = Instantiate(m_heroPrefab);
            hero.Initialize(m_nextHeroId, core, m_worldState);
            hero.Movement.TeleportToLoation(location);
            hero.Destroyed += RetireHero;
            m_createdHeroes.Add(core, hero);

            m_nextHeroId++;
            
            HeroCreated?.Invoke(this, hero);

            return true;
        }
    }
}
