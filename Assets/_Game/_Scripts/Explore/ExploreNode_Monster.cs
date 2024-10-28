using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using XNode;

namespace Game.Exploration
{
    [NodeWidth(300)]
    public class ExploreNode_Monster : ExploreNode
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : ExploreNodeState
        {
            public float targetTimeInState;
            public float elapsedTimeInState;

            public State(ExploreNode node) : base(node) { }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private MonsterHunt.MonsterPopulation[] m_monsters;


        [Input] public ExplorePort entry;
        [Output(dynamicPortList = true)] public ExplorePort[] exits;

        public override IExploreNodeState Enter(IExploreState bb)
        {
            throw new System.NotImplementedException();
        }

        public override void OnWorldTimeUpdate(IExploreState bb, float deltaSeconds)
        {
            throw new System.NotImplementedException();
        }

        protected override ExplorePort GetExplorePort(NodePort nodePort)
        {
            if (nodePort.fieldName == nameof(entry))
                return entry;

            if (nodePort.fieldName == nameof(exits))
                return null;

            if (exits.Length == 1)
                return exits[0];

            int index = int.Parse(nodePort.fieldName.Substring(nameof(exits).Length + 1));
            return exits[index];
        }
    } 
}
