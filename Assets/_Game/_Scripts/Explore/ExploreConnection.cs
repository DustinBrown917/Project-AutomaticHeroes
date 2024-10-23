using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreConnection : MonoBehaviour
    {
        [field: SerializeField] public ExploreNode NodeA { get; private set; }
        [field: SerializeField] public ExploreNode NodeB { get; private set; }

        public IReadOnlyList<SkillCheck> SkillChecks => m_skillChecks;
        [SerializeField] private SkillCheck[] m_skillChecks;

        public bool HeroCanAccessConnection(Hero hero)
        {
            if (m_skillChecks == null || m_skillChecks.Length == 0)
                return true;

            for(int i = 0; i < m_skillChecks.Length; i++) {
                if (m_skillChecks[i].HeroPassesCheck(hero, WorldState.Instance)) {
                    return true;
                }
            }

            return false;
        }
    }

    public static class ConnectionExtensions
    {
        public static bool IsValid(this ExploreConnection connection)
        {
            return connection != null && connection.NodeA != null && connection.NodeB != null && connection.NodeA != connection.NodeB;
        }
    }

    public class ConnectionToNeighbourA : IConnectionToNeighbour
    {
        private ExploreConnection m_connection;

        public ExploreNode Neighbour => m_connection.NodeA;

        public IReadOnlyList<SkillCheck> SkillChecks => m_connection.SkillChecks;

        public ConnectionToNeighbourA(ExploreConnection connection)
        {
            m_connection = connection;
        }

        public bool HeroCanAccessConnection(Hero hero) => m_connection.HeroCanAccessConnection(hero);
    }

    public class ConnectionToNeighbourB : IConnectionToNeighbour
    {
        private ExploreConnection m_connection;

        public ExploreNode Neighbour => m_connection.NodeB;

        public IReadOnlyList<SkillCheck> SkillChecks => m_connection.SkillChecks;

        public ConnectionToNeighbourB(ExploreConnection connection)
        {
            m_connection = connection;
        }

        public bool HeroCanAccessConnection(Hero hero) => m_connection.HeroCanAccessConnection(hero);
    }

    public interface IConnectionToNeighbour
    {
        public ExploreNode Neighbour { get;  }

        public IReadOnlyList<SkillCheck> SkillChecks { get; }

        public bool HeroCanAccessConnection(Hero hero);
    }
}
