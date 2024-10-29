using SegoLib;
using System;

namespace Game.Flow
{
    public interface IFlowState : SegoLib.IDisposable
    {
        public enum EStatus : byte
        {
            Active,
            Concluded
        }

        public WorldState WorldState { get; }
        public Hero Hero { get; }
        public float ElapsedTime { get; }
        public IFlowNodeState NodeState { get; }
        public EStatus Status { get; }

        public string FlavourText { get; set; }
        public event ValueUpdatedWithSourceEvent<IFlowState, string> flavourTextUpdated;

        public void EnterNode(FlowNode node);
        public void OnWorldTimeUpdate(float deltaSeconds);
    }

    public class FlowState : IFlowState
    {
        public WorldState WorldState { get; private set; }

        public Hero Hero { get; private set; }

        public IFlowState.EStatus Status { get; private set; }

        public float ElapsedTime { get; private set; }

        public IFlowNodeState NodeState { get; private set; }

        public string FlavourText
        {
            get => m_flavourText;
            set => SetFlavourText(value);
        }
        private string m_flavourText;
        public event ValueUpdatedWithSourceEvent<IFlowState, string> flavourTextUpdated;

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> disposed;

        public FlowState(WorldState worldState, Hero hero, FlowNode node)
        {
            WorldState = worldState;
            Hero = hero;
            EnterNode(node);
        }

        public void OnWorldTimeUpdate(float deltaSeconds)
        {
            if (NodeState.IsConcluded) {
                if (NodeState.NextNode != null) {
                    EnterNode(NodeState.NextNode);
                } else {
                    Status = IFlowState.EStatus.Concluded;
                    OnStateConcluded();
                    return;
                }
            }

            ElapsedTime += deltaSeconds;
            NodeState.Node.OnWorldTimeUpdate(this, deltaSeconds);
        }


        public void EnterNode(FlowNode node)
        {
            IFlowNodeState oldState = NodeState;
            NodeState = node.Enter(this);
            oldState?.Dispose();
        }

        public void SetFlavourText(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
                newValue = string.Empty;

            if (newValue == m_flavourText)
                return;

            m_flavourText = newValue;

            flavourTextUpdated?.Invoke(this, newValue);
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            OnDisposed();

            WorldState = null;
            Hero = null;

            if (NodeState != null)
                NodeState.Dispose();

            NodeState = null;

            disposed?.Invoke(this);
            disposed = null;
        }


        protected virtual void OnStateConcluded() { }
        protected virtual void OnDisposed() { }
    }
}
