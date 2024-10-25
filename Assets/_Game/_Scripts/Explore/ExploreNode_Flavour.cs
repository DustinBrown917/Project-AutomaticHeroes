using SegoLib;
using System;
using UnityEngine;
using XNode;

namespace Game.Exploration
{
    [NodeWidth(300)]
	public class ExploreNode_Flavour : ExploreNode
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



        [Input] public ExplorePort entry;

        [SerializeField] private FloatRange m_timeToSpendInNode = new FloatRange(900, 4500);
        [SerializeField] private string[] m_flavours;

        [Output(dynamicPortList = true)] public ExplorePort[] exits;

        // Use this for initialization
        protected override void Init()
		{
			base.Init();
		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
            if (exits.Length == 1)
                return exits[0];

			int index = int.Parse(port.fieldName.Substring(nameof(exits).Length + 1));
			return exits[index];
		}


        public override IExploreNodeState Enter(IExploreState bb)
        {
            State state = new State(this);
            state.targetTimeInState = bb.WorldState.Rng.Range(m_timeToSpendInNode);
            bb.FlavourText = bb.WorldState.Rng.GetRandom(m_flavours);
            return state;
        }

        public override void OnWorldTimeUpdate(IExploreState bb, float deltaSeconds)
        {
            if(bb.CurrentExploreNodeState is not State state) {
                return;
            }

            state.elapsedTimeInState += deltaSeconds;

            bool hasStamina = bb.Hero.Stamina.SpendEnergy(HeroStamina.DEFAULT_STAMINA_PER_SECOND * deltaSeconds);

            if(!hasStamina || state.elapsedTimeInState >= state.targetTimeInState) {
                state.Resolve(new NodeResolution(null, hasStamina));
            }
        }

    } 
}