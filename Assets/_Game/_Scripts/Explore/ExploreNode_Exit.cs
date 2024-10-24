using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Game.Exploration
{
	public class ExploreNode_Exit : ExploreNode
	{
        protected class State : IExploreNodeState
        {
            public bool IsResolved { get; set; }

            public ExploreNode Node { get; set; }

            public NodeResolution Resolution { get; set; }
        }


        // Use this for initialization
        protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			return null; // Replace this
		}

        public override IExploreNodeState Enter(IExploreState bb)
        {
            return new State();
        }

        public override void OnWorldTimeUpdate(IExploreState bb, float deltaSeconds)
        {
            throw new System.NotImplementedException();
        }
    } 
}