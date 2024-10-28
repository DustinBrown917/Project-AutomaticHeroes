using SegoLib;
using System;

namespace Game.Monsters.Combat
{
    public interface INodeState : SegoLib.IDisposable
    {
        public Node Node { get; }
        public bool IsConcluded { get; }
        public Node NextNode { get; }
    }

    public class NodeState : INodeState
    {
        public Node Node { get; private set; }

        public bool IsConcluded { get; private set; }

        public Node NextNode { get; private set; }

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> disposed;

        public NodeState(Node node)
        {
            this.Node = node;
        }

        public void Conclude(Node nextNode)
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
