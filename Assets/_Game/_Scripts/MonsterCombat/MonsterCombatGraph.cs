using SegoLib;
using System;
using UnityEngine;
using XNode;
using Game.Flow;

namespace Game.Monsters.Combat
{
    [CreateAssetMenu(fileName = "New Monster Combat Graph", menuName = "Game/Graphs/Monster Combat")]
    public class MonsterCombatGraph : NodeGraph
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        private class State : FlowState, IMonsterCombatState
        {
            public Monster Monster { get; private set; }
            public bool CombatIsConcluded { get; private set; }
            public IMonsterCombatState.ECombatResult Result { get; set; }

            public State(WorldState worldState, Hero hero, Monster monster, MonsterCombatNode node) : base(worldState, hero, node)
            {
                Monster = monster;
            }

            protected override void OnDisposed()
            {
                Monster = null;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        private MonsterCombatNode_Enter m_entryNode;

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
