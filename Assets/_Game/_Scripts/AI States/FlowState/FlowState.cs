using SegoLib;
using System;

namespace Game.Flow
{
    public interface IFlowState : SegoLib.IDisposable
    {
    }

    public class FlowState : IFlowState
    {
        public bool EndOfFlowReached { get; private set; }

        public FlowNode CurrentNode { get; private set; }

        public object NodeData { get; private set; }

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> Disposed;

        public FlowState(FlowNode node)
        {
            EnterNode(node);
        }

        private void EnterNode(FlowNode node)
        {
            CurrentNode = node;
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            OnDisposed();

            CurrentNode = null;

            Disposed?.Invoke(this);
            Disposed = null;
        }

        protected virtual void OnDisposed() { }
    }
}
