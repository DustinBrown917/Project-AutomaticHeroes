using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class ExploreNode : MonoBehaviour, IExploreNode
    {
        public ExploreRoot Root { get; private set; }

        public IReadOnlyList<IConnectionToNeighbour> Connections => m_connections;
        private List<IConnectionToNeighbour> m_connections = new();

        [field: SerializeField] public bool IsEntranceNode { get; private set; }

        public bool Initialized { get; private set; }

        public void Initialize(ExploreRoot root)
        {
            if(Initialized) return;

            Root = root;

            RefreshConnections(root);

            Initialized = true;
        }

        public void RefreshConnections(ExploreRoot root)
        {
            m_connections.Clear();

            foreach(ExploreConnection connection in root.Connections) {
                if(!connection.IsValid()) {
                    continue;
                }

                if (connection.NodeA == this)
                    m_connections.Add(new ConnectionToNeighbourB(connection));
                else if(connection.NodeB == this)
                    m_connections.Add(new ConnectionToNeighbourA(connection));
            }
        }

        public void Enter(IExploreState state)
        {
            OnEnter(state);
        }

        public void OnWorldTimeUpdate(IExploreState state, float deltaSeconds)
        {
            OnUpdate(state, deltaSeconds);
        }

        protected abstract void OnEnter(IExploreState state);
        protected abstract void OnUpdate(IExploreState state, float deltaSeconds);
    }

    public interface IExploreNode
    {
        public ExploreRoot Root { get; }
        public IReadOnlyList<IConnectionToNeighbour> Connections { get; }
        public bool IsEntranceNode { get; }
        public bool Initialized { get; }
        public void Initialize(ExploreRoot root);
        public abstract void Enter(IExploreState state);
        public abstract void OnWorldTimeUpdate(IExploreState state, float deltaSeconds);
    }
}
