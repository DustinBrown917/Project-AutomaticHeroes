using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploreRoot : MonoBehaviour
    {
        [SerializeField] private ExploreNode[] m_nodes;

        public IReadOnlyList<ExploreNode> EntranceNodes => m_entranceNodes;
        private ExploreNode[] m_entranceNodes;

        public IReadOnlyList<ExploreConnection> Connections => m_connections;
        [SerializeField, ReadOnly] private ExploreConnection[] m_connections;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if(Initialized) return;

            CollectNodes();
            CollectConnections();
            UpdateNodeConnections();

            for(int i = 0; i < m_nodes.Length; i++) {
                m_nodes[i].Initialize(this);
            }

            Initialized = true;
        }

        private void CollectNodes()
        {
            m_nodes = GetComponentsInChildren<ExploreNode>();
        }

        private void CollectConnections()
        {
            List<ExploreConnection> connections = new List<ExploreConnection>(GetComponentsInChildren<ExploreConnection>());
            for(int i = connections.Count - 1; i >= 0; i--) {
                if (!connections[i].IsValid()) {
                    connections.RemoveAt(i);
                }
            }
            m_connections = connections.ToArray();
        }

        private void UpdateNodeConnections()
        {
            foreach (ExploreNode node in m_nodes) {
                node.RefreshConnections(this);
            }
        }
        private void FindEntranceNodes()
        {
            List<ExploreNode> entranceNodes = UnityEngine.Pool.ListPool<ExploreNode>.Get();

            for(int i = 0; i < m_nodes.Length; i++) {
                if (m_nodes[i].IsEntranceNode)
                    entranceNodes.Add(m_nodes[i]);
            }

            if(entranceNodes.Count == 0) {
                UnityEngine.Debug.LogWarning($"No entrance nodes found in {nameof(ExploreRoot)} on {gameObject.name}. Defaulting to first node.", gameObject);
                entranceNodes.Add(m_nodes[0]);
            }

            m_entranceNodes = entranceNodes.ToArray();

            UnityEngine.Pool.ListPool<ExploreNode>.Release(entranceNodes);
        }

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== EDITOR ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [Button]
        private void Refresh()
        {
            CollectNodes();
            CollectConnections();
            UpdateNodeConnections();
        }

        private void OnDrawGizmos()
        {
            if (m_connections == null || m_connections.Length < 0)
                Refresh();

            Color originalGizmosColor = Gizmos.color;

            for(int i = 0; i < m_connections.Length; i++) {
                if (m_connections[i] == null) {
                    CollectConnections();
                    return;
                }

                if (m_connections[i].NodeA == null || m_connections[i].NodeB == null) 
                    continue;

                Gizmos.color = Color.green;
                DrawArrow(m_connections[i].NodeA.transform.position, m_connections[i].NodeB.transform.position, 0.15f, true);
                UnityEditor.Handles.Label(Vector3.Lerp(m_connections[i].NodeA.transform.position, m_connections[i].NodeB.transform.position, 0.5f), i.ToString());
            }

            Gizmos.color = originalGizmosColor;
        }

        private void DrawArrow(Vector2 from, Vector2 to, float arrowWingLength = 0.1f, bool bidirectional = false)
        {
            const float WING_FLARE = 0.4f;

            Gizmos.DrawLine(from, to);
            Vector2 direction = (to - from).normalized;
            Vector2 crossPoint = to - (direction * arrowWingLength);
            Vector2 wingPoint1 = crossPoint + (new Vector2(direction.y, -direction.x) * (arrowWingLength * WING_FLARE));
            Vector2 wingPoint2 = crossPoint + (new Vector2(-direction.y, direction.x) * (arrowWingLength * WING_FLARE));
            Gizmos.DrawLine(to, wingPoint1);
            Gizmos.DrawLine(to, wingPoint2);
            Gizmos.DrawLine(wingPoint1, wingPoint2);

            if (bidirectional) {
                crossPoint = from + (direction * arrowWingLength);
                wingPoint1 = crossPoint + (new Vector2(direction.y, -direction.x) * (arrowWingLength * WING_FLARE));
                wingPoint2 = crossPoint + (new Vector2(-direction.y, direction.x) * (arrowWingLength * WING_FLARE));
                Gizmos.DrawLine(from, wingPoint1);
                Gizmos.DrawLine(from, wingPoint2);
                Gizmos.DrawLine(wingPoint1, wingPoint2);
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End EDITOR ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

    } 
}
