using Game.Flow;
using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Game.Exploration
{
    [RequireNode(typeof(ExploreNode_Enter), typeof(ExploreNode_Exit))]
	[CreateAssetMenu(fileName = "New Explore Graph", menuName = "Game/Explore Graph")]
	public class ExploreGraph : FlowGraph
	{
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        private class State : FlowState, IExploreState
        {
            public State(WorldState worldState, Hero hero, ExploreNode node) : base(worldState, hero, node)
            {
            }

            protected override void OnDisposed()
            {
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [System.NonSerialized] private ExploreNode_Enter[] m_entries;

        protected override void OnInitialize()
        {
            m_entries = FindEntries();
        }

        private ExploreNode_Enter[] FindEntries()
        {
            List<ExploreNode_Enter> entries = UnityEngine.Pool.ListPool<ExploreNode_Enter>.Get();

            foreach(Node node in nodes) {
                if(node is ExploreNode_Enter entryNode)
                    entries.Add(entryNode);
            }

            ExploreNode_Enter[] entriesArray = entries.ToArray();
            UnityEngine.Pool.ListPool<ExploreNode_Enter>.Release(entries);
            return entriesArray;
        }

        public IExploreState Enter(WorldState worldState, Hero hero)
        {
            ExploreNode_Enter entry = worldState.Rng.GetRandom(m_entries);

            State newState = new State(worldState, hero, entry);

            return newState;
        }
    }
}