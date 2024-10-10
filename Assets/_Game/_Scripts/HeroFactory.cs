using UnityEngine;

namespace Game
{
    public class HeroFactory : MonoBehaviour
    {
        public static Hero CreateHero(ILocation location)
        {
            Hero hero = Instantiate(m_instance.m_heroPrefab.gameObject).GetComponent<Hero>();
            hero.Initialize(m_instance.m_heroColors[m_instance.m_nextHeroColor++], location);

            if(m_instance.m_nextHeroColor >= m_instance.m_heroColors.Length) {
                m_instance.m_nextHeroColor = 0;
            }

            return hero;
        }

        private static HeroFactory m_instance;

        [SerializeField] private Hero m_heroPrefab;
        [SerializeField] private HeroColor[] m_heroColors;

        private int m_nextHeroColor;

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
        }
    }
}
