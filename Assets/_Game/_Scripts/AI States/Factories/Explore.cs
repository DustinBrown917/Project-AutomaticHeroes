using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Explore : HeroBehaviour
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== State ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.State, IExploreState
        {
            string IExploreState.FlavourText {
                get => FlavourText;
                set => FlavourText = value;
            }

            public float ElapsedTimeInCurrentNode { get; set; } = 0;
            public bool CurrentNodeIsDone { get; set; }

            public ExploreNode currentNode;

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero) : base(behaviour, worldState, hero)
            {
            }


        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End State ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Behaviour ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class Behaviour : IHeroBehaviour.Behaviour<State>
        {
            private ExploreRoot m_exploreRoot;

            public bool Initialized { get; private set; }

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour)
            {
            }

            public void Initialize(ExploreRoot exploreRoot)
            {
                m_exploreRoot = exploreRoot;
                exploreRoot.Initialize();
            }

            protected override void OnBegin(State bb)
            {
                EnterNode(bb, bb.WorldState.Rng.GetRandom(m_exploreRoot.EntranceNodes));
            }

            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                if (bb.CurrentNodeIsDone) {
                    // Get next node from connections.
                }

                bb.ElapsedTimeInCurrentNode += deltaSeconds;
                bb.currentNode.OnWorldTimeUpdate(bb, deltaSeconds);
            }

            protected override State CreateNewState(Hero hero)
            {
                State newState = new(ParentBehaviour, WorldState.Instance, hero);
                return newState;
            }

            private void EnterNode(State bb, ExploreNode node)
            {
                if (bb.currentNode == node)
                    return;

                bb.ElapsedTimeInCurrentNode = 0;
                bb.currentNode = node;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Behaviour ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private Behaviour m_behaviour;
        [SerializeField] private ExploreRoot m_exploreRootPrefab;

        public Explore()
        {
            m_behaviour = new Behaviour(this);
        }

        public override bool CanHeroDoBehaviour(Hero hero)
        {
            throw new System.NotImplementedException();
        }

        public override float GetPriority(Hero hero)
        {
            throw new System.NotImplementedException();
        }

        protected override IHeroBehaviour.IBehaviour InitializeBehaviour()
        {
            ExploreRoot exploreRoot = Object.Instantiate(m_exploreRootPrefab.gameObject).GetComponent<ExploreRoot>();
            m_behaviour.Initialize(exploreRoot);

            return m_behaviour;
        }
    }
}
