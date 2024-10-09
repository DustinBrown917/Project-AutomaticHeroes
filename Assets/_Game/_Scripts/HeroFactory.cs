using UnityEngine;

namespace Game
{
    public class HeroFactory : MonoBehaviour
    {
        public static Hero CreateHero(ILocation location)
        {
            Hero hero = Instantiate(m_instance.m_heroPrefab.gameObject).GetComponent<Hero>();
            hero.Initialize(location);

            return hero;
        }

        private static HeroFactory m_instance;

        [SerializeField] private Hero m_heroPrefab;

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
