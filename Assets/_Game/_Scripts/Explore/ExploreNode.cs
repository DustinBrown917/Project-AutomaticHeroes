using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Game.Exploration
{

    public abstract class ExploreNode : Node
    {
        [System.Flags]
        public enum EEntryExitType : byte
        {
            None = 0,
            Entry = 1,
            Exit = 2,
            Both = Entry | Exit
        }

        new public ExploreGraph graph { get; private set; }
        [field:SerializeField] public string Name { get; private set; }
        public bool IsEntryNode => NodeType.HasFlag(EEntryExitType.Entry);
        public bool IsExitNode => NodeType.HasFlag(EEntryExitType.Exit);
        [field: SerializeField] public EEntryExitType NodeType { get; private set; }


        public IReadOnlyList<ExplorePort> ConnectedOutputs => m_connectedOutputs;
        [System.NonSerialized] private readonly List<ExplorePort> m_connectedOutputs = new();


        protected override void Init()
        {
            base.Init();

            graph = (ExploreGraph)base.graph;

            foreach(NodePort port in Outputs) {
                if (port.ConnectionCount == 0)
                    continue;

                ExplorePort explorePort = GetExplorePort(port);

                if (explorePort == null)
                    return;

                if(port.Connection.node is ExploreNode otherNode) {
                    explorePort.SetConnectedNode(otherNode);
                } else {
                    return;
                }

                m_connectedOutputs.Add(explorePort);
            }
        }

        public override object GetValue(NodePort port)
        {
            return GetExplorePort(port);
        }

        public abstract IExploreNodeState Enter(IExploreState bb);
        public abstract void OnWorldTimeUpdate(IExploreState bb, float deltaSeconds);

        protected abstract ExplorePort GetExplorePort(NodePort nodePort);
    }

    public readonly struct NodeResolution
    {
        public readonly HeroSkillDef skillUsed;
        public readonly bool wasSuccessful;

        public NodeResolution(HeroSkillDef skillUsed, bool wasSuccessful)
        {
            this.skillUsed = skillUsed;
            this.wasSuccessful = wasSuccessful;
        }
    }



    [System.Serializable]
    public class ExplorePort
    {
        public enum EResolutionRequirement : byte
        {
            Any,
            Failure,
            Success
        }

        [SerializeField] private string m_name;
        [field: SerializeField] public EResolutionRequirement ResolutionRequirement { get; private set; }
        [field: SerializeField] public HeroSkillDef SkillUsed { get; private set; }
        [field: SerializeField] public float Weight { get; private set; }
        [field: System.NonSerialized] public ExploreNode ConnectedNode { get; private set; }

        public void SetConnectedNode(ExploreNode node)
        {
            ConnectedNode = node;
        }

        public bool IsValidForResolution(in NodeResolution resolution)
        {
            if (ResolutionRequirement == EResolutionRequirement.Failure && resolution.wasSuccessful)
                return false;

            if (ResolutionRequirement == EResolutionRequirement.Success && !resolution.wasSuccessful)
                return false;

            if (SkillUsed != null && resolution.skillUsed != SkillUsed)
                return false;

            return true;
        }
    }
}
