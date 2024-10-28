using SegoLib;
using System;
using UnityEngine;
using XNode;

namespace Game.Monsters.Combat
{
    [CreateAssetMenu(fileName = "New Monster Combat Graph", menuName = "Game/Graphs/Monster Combat")]
    public class Graph : NodeGraph
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        private class State : IState
        {
            public WorldState WorldState { get; private set; }

            public Hero Hero { get; private set; }

            public Monster Monster { get; private set; }

            public float ElapsedTime { get; private set; }

            public INodeState NodeState { get; set; }

            public bool CombatIsConcluded { get; private set; }
            public IState.ECombatResult Result { get; set; }

            public string FlavourText
            {
                get => m_flavourText;
                set => SetFlavourText(value);
            }
            private string m_flavourText;
            public event ValueUpdatedWithSourceEvent<IState, string> flavourTextUpdated;

            public bool IsDisposed { get; private set; }
            public event Action<IPrivatelyDisposable> disposed;

            public State(WorldState worldState, Hero hero, Monster monster, Node node)
            {
                WorldState = worldState;
                Hero = hero;
                Monster = monster;
                EnterNode(node);
            }

            public void OnWorldTimeUpdate(float deltaSeconds)
            {
                if (NodeState.IsConcluded) {
                    if(NodeState.NextNode != null) {
                        EnterNode(NodeState.NextNode);
                    } else {
                        CombatIsConcluded = true;
                        return;
                    }
                }

                ElapsedTime += deltaSeconds;
                NodeState.Node.OnWorldTimeUpdate(this, deltaSeconds);
            }

            public void EnterNode(Node node)
            {
                INodeState oldState = NodeState;
                NodeState = node.Enter(this);
                oldState?.Dispose();
            }

            public void SetFlavourText(string newValue)
            {
                if(string.IsNullOrEmpty(newValue))
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

                WorldState = null;
                Hero = null;
                Monster = null;

                if(NodeState != null)
                    NodeState.Dispose();

                NodeState = null;

                disposed?.Invoke(this);
                disposed = null;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        private Node_Enter m_entryNode;

        [field: NonSerialized] public bool Initialized { get; private set; }


        public void Initialize()
        {
            if (Initialized)
                return;

            if(m_entryNode == null) {
                m_entryNode = FindEntryNode();
            }

            Initialized = true;
        }

        public IState Enter(WorldState worldState, Hero hero, Monster monster)
        {
            State newState = new(worldState, hero, monster, m_entryNode);
            return newState;
        }

        private Node_Enter FindEntryNode()
        {
            foreach(Node node in nodes) {
                if(node is Node_Enter entryNode) {
                    return entryNode;
                }
            }

            return null;
        }
    } 
}
