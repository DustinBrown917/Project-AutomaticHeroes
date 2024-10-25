using SegoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Exploration
{
    public interface IExploreState
    {
        public Hero Hero { get; }
        public WorldState WorldState { get; }
        public float ElapsedTime { get; }
        public string FlavourText { get; set; }
        public IExploreNodeState CurrentExploreNodeState { get; }
    }

    public interface IExploreNodeState : SegoLib.IDisposable
    {
        public bool IsResolved { get; }
        public ExploreNode Node { get; }
        public NodeResolution Resolution { get; }
    }

    public abstract class ExploreNodeState : IExploreNodeState
    {
        public bool IsResolved { get; private set; }

        public ExploreNode Node { get; private set; }

        public NodeResolution Resolution { get; private set; }

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> disposed;

        public ExploreNodeState(ExploreNode node)
        {
            this.Node = node;
        }

        public void Resolve(NodeResolution resolution)
        {
            if (IsResolved) {
                UnityEngine.Debug.LogWarning($"Attempting to resolve an already resolved node state. Node: {Node}, State: {GetType().Name}");
                return;
            }
            IsResolved = true;
            Resolution = resolution;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            OnDisposed();

            Node = null;
            Resolution = new NodeResolution();

            disposed?.Invoke(this);
            disposed = null;
        }

        protected virtual void OnDisposed() { }
    }
}
