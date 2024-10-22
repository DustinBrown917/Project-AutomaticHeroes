using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreNode : MonoBehaviour
    {
        public IReadOnlyList<ExploreConnection> Connetions => m_connections;
        [SerializeField] private ExploreConnection[] m_connections;

        [Button]
        public void UpdateConnections()
        {
            m_connections = GetComponents<ExploreConnection>();
        }
    } 
}
