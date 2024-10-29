using SegoLib;
using System;

namespace Game.Flow
{
    public interface IFlowNodeState : SegoLib.IDisposable
    {
        public FlowNode Node { get; }
        public bool IsConcluded { get; }
        public FlowNode NextNode { get; }
    }

    public class FlowNodeState : IFlowNodeState
    {
        public FlowNode Node { get; private set; }

        public bool IsConcluded { get; private set; }

        public FlowNode NextNode { get; private set; }

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> disposed;

        public FlowNodeState(FlowNode node)
        {
            this.Node = node;
        }

        public void Conclude(FlowNode nextNode)
        {
            NextNode = nextNode;
            IsConcluded = true;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            OnDisposed();

            Node = null;

            disposed?.Invoke(this);
            disposed = null;
        }

        protected virtual void OnDisposed() { }
    }
}
