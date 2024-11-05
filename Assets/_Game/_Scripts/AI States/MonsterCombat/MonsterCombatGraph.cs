using SegoLib;
using System;
using UnityEngine;
using Game.Flow;

namespace Game.Monsters.Combat
{
    [RequireNode(typeof(MonsterCombatNode_Enter), typeof(MonsterCombatNode_Exit))]
    [CreateAssetMenu(fileName = "New Monster Combat Graph", menuName = "Game/Graphs/Monster Combat")]
    public class MonsterCombatGraph : FlowGraph
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        private class State : IMonsterCombatState
        {
            public WorldState WorldState { get; private set; }
            public Hero Hero { get; private set; }
            public Monster Monster { get; private set; }
            public MonsterCombat Combat { get; private set; }
            public float ElapsedTime { get; private set; }
            public float NodeEndTime { get; set; }
            public bool CombatIsConcluded { get; private set; }
            public IMonsterCombatState.ECombatResult Result { get; set; }

            public string FlavourText
            {
                get => m_flavourText;
                set => SetFlavourText(value);
            }
            private string m_flavourText;
            public event ValueUpdatedWithSourceEvent<IMonsterCombatState, string> FlavourTextUpdated;

            private MonsterCombatNode m_currentNode;

            public bool IsDisposed { get; private set; }
            public event Action<IPrivatelyDisposable> Disposed;


            public State(WorldState worldState, Hero hero, Monster monster, MonsterCombatNode node)
            {
                WorldState = worldState;
                Hero = hero;
                Monster = monster;
                Combat = new MonsterCombat(worldState, hero, monster);
                m_currentNode = node;
            }


            public void ConcludeCurrentNode(MonsterCombatNode nextNode)
            {
                m_currentNode = nextNode;
                m_currentNode.Enter(this);

                if (m_currentNode == null)
                    CombatIsConcluded = true;
            }

            public void OnWorldTimeUpdate(float deltaSeconds)
            {
                ElapsedTime += deltaSeconds;

                if(m_currentNode != null) {
                    m_currentNode.OnWorldTimeUpdate(this, deltaSeconds);
                }
            }

            public void SetFlavourText(string newValue)
            {
                if (newValue == null)
                    newValue = string.Empty;

                if (newValue == m_flavourText)
                    return;

                m_flavourText = newValue;

                FlavourTextUpdated?.Invoke(this, m_flavourText);
            }

            public void Dispose()
            {
                if(IsDisposed) return;
                IsDisposed = true;

                Combat.Dispose();
                Combat = null;
                Monster = null;
                WorldState = null;
                Hero = null;

                Disposed?.Invoke(this);
                Disposed = null;
            }


        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        private MonsterCombatNode_Enter m_entryNode;

        protected override void OnInitialize()
        {
            if(m_entryNode == null) {
                m_entryNode = FindEntryNode();
            }
        }

        public IMonsterCombatState Enter(WorldState worldState, Hero hero, Monster monster)
        {
            State newState = new(worldState, hero, monster, m_entryNode);
            return newState;
        }

        private MonsterCombatNode_Enter FindEntryNode()
        {
            foreach(MonsterCombatNode node in nodes) {
                if(node is MonsterCombatNode_Enter entryNode) {
                    return entryNode;
                }
            }

            return null;
        }
    } 
}
