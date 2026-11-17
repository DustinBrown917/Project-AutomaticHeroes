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


        public event HeroCreatedEvent HeroCreated;
        public event HeroRetiredEvent HeroRetired;

        private void OnDestroy()
        {
            foreach(Hero hero in m_createdHeroes.Values) {
                if (hero != null)
                    hero.Destroyed -= RetireHero;
            }

            m_createdHeroes.Clear();
            m_heroPrefab = null;
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
            hero.Initialize(m_nextHeroId, core);
            hero.Movement.TeleportToLoation(location);
            hero.Destroyed += RetireHero;
            m_createdHeroes.Add(core, hero);

            m_nextHeroId++;
            
            HeroCreated?.Invoke(this, hero);

            return true;
        }
    }
}
