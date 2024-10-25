using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Game.Exploration
{
	[CreateAssetMenu(fileName = "New Explore Graph", menuName = "Game/Explore Graph")]
	public class ExploreGraph : NodeGraph
	{
		public IReadOnlyList<ExploreNode> EntryNodes => m_entryNodes;
		private readonly List<ExploreNode> m_entryNodes = new();

		public IReadOnlyList<ExploreNode> ExploreNodes => m_exploreNodes;
		private readonly List<ExploreNode> m_exploreNodes = new();

		public IReadOnlyList<ExploreNode> ExitNodes => m_exitNodes;
		private readonly List<ExploreNode> m_exitNodes = new();

		[field: SerializeField] public ExploreNode DeadEndExitNode { get; private set; }

		[field: NonSerialized] public bool Initialized { get; private set; }
		public void Initialize()
		{
			if (Initialized)
				return;

			InitExploreNodes();

			Initialized = true;
		}

		private void InitExploreNodes()
		{
			foreach(Node node in nodes) {
				if(node is ExploreNode exploreNode) {
					m_exploreNodes.Add(exploreNode);
					if (exploreNode.IsEntryNode) {
						m_entryNodes.Add(exploreNode);
					}
					if (exploreNode.IsExitNode) {
						m_exitNodes.Add(exploreNode);
					}
				}
			}
		}
	} 
}