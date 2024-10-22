using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreRoot : MonoBehaviour
    {
        [SerializeField] private ExploreNode[] m_nodes;

        [Button]
        public void Refresh()
        {
            m_nodes = GetComponentsInChildren<ExploreNode>();
            foreach(ExploreNode node in m_nodes) {
                node.UpdateConnections();
            }
        }

        private void OnDrawGizmos()
        {
            if (m_nodes == null || m_nodes.Length == 0)
                return;

            HashSet<ExploreConnection> drawnConnections = UnityEngine.Pool.HashSetPool<ExploreConnection>.Get();

            Color originalGizmosColor = Gizmos.color;

            foreach(ExploreNode node in m_nodes) {
                foreach(ExploreConnection connection in node.Connetions) {
                    if (!drawnConnections.Add(connection))
                        continue;

                    bool isBidirectionalConnection = false;
                    foreach(ExploreConnection connection2 in connection.ConnectedNode.Connetions) {
                        if(connection2.ConnectedNode == node) {
                            isBidirectionalConnection = true;
                            drawnConnections.Add(connection2);
                        }
                    }

                    if (isBidirectionalConnection)
                        Gizmos.color = Color.white;
                    else if (connection.ConnectedNode.transform.position.y < node.transform.position.y)
                        Gizmos.color = Color.green;
                    else
                        Gizmos.color = Color.yellow;

                    Gizmos.DrawLine(node.transform.position, connection.ConnectedNode.transform.position);
                }
            }

            Gizmos.color = originalGizmosColor;

            UnityEngine.Pool.HashSetPool<ExploreConnection>.Release(drawnConnections);
        }
    } 
}
