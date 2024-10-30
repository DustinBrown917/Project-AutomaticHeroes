using System;
using System.Collections.Generic;
using XNode;

namespace Game.Flow
{
    [System.Serializable]
    public class FlowPort
    {
        [field: NonSerialized] public FlowNode Node { get; private set; }

        public FlowPort ConnectedPortData => m_connectedPorts.Length > 0 ? m_connectedPorts[0] : null;

        public IReadOnlyList<FlowPort> ConnectedPorts => m_connectedPorts;
        [NonSerialized] private FlowPort[] m_connectedPorts;

        [field: NonSerialized] public NodePort WrappedPort { get; private set; }

        public void SetWrappedPort(NodePort port)
        {
            if (port == null)
                return;

            WrappedPort = port;

            if (port.node is not FlowNode node)
                return;

            Node = node;

            if (port.ConnectionCount == 0)
                return;

            List<FlowPort> connections = UnityEngine.Pool.ListPool<FlowPort>.Get();

            for (int i = 0; i < port.ConnectionCount; i++) {
                NodePort connectedPort = port.GetConnection(i);

                if (connectedPort.node is not FlowNode connectedNode)
                    continue;

                FlowPort connection = connectedNode.GetFlowPort(connectedPort);

                if (connection != null)
                    connections.Add(connection);
            }

            if (connections.Count > 0)
                m_connectedPorts = connections.ToArray();
            else
                m_connectedPorts = Array.Empty<FlowPort>();

            UnityEngine.Pool.ListPool<FlowPort>.Release(connections);
        }
    }

}