using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroFactory : MonoBehaviour
    {
        private static HeroFactory m_instance;

        public static IReadOnlyList<Hero> AllCreatedHeroes => m_instance.m_createdHeroes;

        public static Hero CreateHero(ILocation location)
        {
            Hero hero = Instantiate(m_instance.m_heroPrefab.gameObject).GetComponent<Hero>();
            hero.Initialize(m_instance.m_nextHeroId, m_instance.m_heroCores[m_instance.GetNextHeroCoreId()]);
            hero.Movement.TeleportToLoation(location);
            m_instance.m_createdHeroes.Add(hero);

            m_instance.m_nextHeroId++;

            return hero;
        }


        [SerializeField] private Hero m_heroPrefab;
        [SerializeField] private HeroCore[] m_heroCores;

        private readonly List<Hero> m_createdHeroes = new();

        private int m_nextHeroId;

        private void Awake()
        {
            if (m_instance != null)
                Destroy(this);
            else
                m_instance = this;
        }

        private void OnDestroy()
        {
            if (m_instance == this)
                m_instance = null;

            m_createdHeroes.Clear();
            m_heroCores = null;
            m_heroPrefab = null;
        }

        private int GetNextHeroCoreId()
        {
            return m_nextHeroId % m_heroCores.Length;
        }
    }
}
