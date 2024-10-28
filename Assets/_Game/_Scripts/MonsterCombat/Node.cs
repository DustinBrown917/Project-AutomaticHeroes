using System.Collections.Generic;
using XNode;

namespace Game.Monsters.Combat
{
    [NodeWidth(300)]
    public abstract class Node : XNode.Node
    {
        new public Graph graph { get; private set; }

        public IReadOnlyList<PortData> ConnectedOutputs => m_connectedOutputs;
        [System.NonSerialized] private readonly List<PortData> m_connectedOutputs = new();

        protected override void Init()
        {
            base.Init();

            graph = (Graph)base.graph;

            InitInputs();

            GatherConnectedOutputs();
        }

        private void InitInputs()
        {
            foreach(NodePort nodePort in Inputs) {
                PortData portData = GetPortData(nodePort);

                if (portData != null)
                    portData.Initialize(this);
            }
        }

        protected void GatherConnectedOutputs()
        {
            foreach (NodePort nodePort in Outputs) {
                if (nodePort.ConnectionCount == 0)
                    continue;

                PortData port = GetPortData(nodePort);

                if (port == null)
                    return;

                port.Initialize(this);

                Node otherNode = nodePort.Connection.node as Node;

                if (otherNode == null)
                    return;

                PortData otherPort = otherNode.GetPortData(nodePort.Connection);

                if(otherPort == null) 
                    return;

                PortData.ConnectPortDatas(port, otherPort);

                m_connectedOutputs.Add(port);
            }
        }

        public override object GetValue(NodePort port)
        {
            return GetPortData(port);
        }

        protected abstract PortData GetPortData(NodePort port);
        public abstract INodeState Enter(IState bb);
        public abstract void OnWorldTimeUpdate(IState bb, float deltaSeconds);
    }
}
