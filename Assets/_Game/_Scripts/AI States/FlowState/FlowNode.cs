using System.Collections.Generic;
using XNode;

namespace Game.Flow
{
    public interface IFlowNode
    {
        public XNode.Node WrappedNode { get; }
    }

    public abstract class FlowNode : XNode.Node
    {
        public static int GetIndexFromFieldName(string root, string fieldName)
        {
            return int.Parse(fieldName.Substring(root.Length));
        }

        public IReadOnlyList<FlowPort> InputData => m_inputs;
        private FlowPort[] m_inputs;
        public IReadOnlyList<FlowPort> OutputData => m_outputs;
        private FlowPort[] m_outputs;

        protected override void Init()
        {
            base.Init();

            InitPorts();
        }

        public void InitPorts()
        {
            List<FlowPort> inputs = UnityEngine.Pool.ListPool<FlowPort>.Get();
            List<FlowPort> outputs = UnityEngine.Pool.ListPool<FlowPort>.Get();

            foreach (NodePort nodePort in Ports) {

                FlowPort portData = GetFlowPort(nodePort);
                if (portData == null)
                    continue;

                if (nodePort.IsInput)
                    inputs.Add(portData);

                if (nodePort.IsOutput)
                    outputs.Add(portData);

                portData.SetWrappedPort(nodePort);
            }

            m_inputs = inputs.ToArray();
            m_outputs = outputs.ToArray();

            UnityEngine.Pool.ListPool<FlowPort>.Release(inputs);
            UnityEngine.Pool.ListPool<FlowPort>.Release(outputs);
        }

        public override object GetValue(NodePort port)
        {
            return GetFlowPort(port);
        }

        public abstract FlowPort GetFlowPort(NodePort port);
    }
}
