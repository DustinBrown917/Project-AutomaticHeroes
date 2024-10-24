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
        public float ElapsedTimeInCurrentNode { get; }
        public string FlavourText { get; set; }
        public IExploreNodeState CurrentExploreNodeState { get; }
    }

    public interface IExploreNodeState
    {
        public bool IsResolved { get; }
        public ExploreNode Node { get; }
        public NodeResolution Resolution { get; }
    }
}
