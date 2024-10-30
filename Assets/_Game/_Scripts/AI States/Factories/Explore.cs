using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Exploration;
using XNode;
using SegoLib.Random;
using Game.Flow;

namespace Game
{
    [System.Serializable]
    public class Explore : HeroBehaviour
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== State ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.State
        {
            public IExploreState ExploreState
            {
                get => m_exploreState;
                set => SetExploreState(value);
            }
            private IExploreState m_exploreState;

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero) : base(behaviour, worldState, hero)
            {
            }


            public void SetExploreState(IExploreState newState)
            {
                if (m_exploreState != null) {
                    m_exploreState.flavourTextUpdated -= CombatState_FlavourTextUpdated;
                    m_exploreState.Dispose();
                }

                m_exploreState = newState;

                if (m_exploreState != null) {
                    m_exploreState.flavourTextUpdated += CombatState_FlavourTextUpdated;
                }
            }

            public void SetFlavourText(string flavourText)
            {
                FlavourText = flavourText;
            }

            private void CombatState_FlavourTextUpdated(IFlowState source, string newValue)
            {
                SetFlavourText(newValue);
            }

            protected override void OnDisposed()
            {
                m_exploreState?.Dispose();
                m_exploreState = null;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End State ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Behaviour ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        protected class Behaviour : IHeroBehaviour.Behaviour<State>
        {

            public ExploreGraph graph;
            public Sprite sprite;
            public string stateString = "Exploring.";


            public bool Initialized { get; private set; }


            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour)
            {
            }

            protected override State CreateNewState(Hero hero)
            {
                if (!graph.Initialized)
                    graph.Initialize();

                State newState = new(ParentBehaviour, WorldState.Instance, hero);
                newState.StateString = stateString;
                newState.ForegroundSprite = sprite;

                newState.ExploreState = graph.Enter(newState.WorldState, newState.Hero);

                return newState;
            }

            public void Initialize()
            {
                graph.Initialize();
            }


            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                bb.ExploreState.OnWorldTimeUpdate(deltaSeconds);

                if(bb.ExploreState.Status == IFlowState.EStatus.Concluded) {
                    bb.ExploreState = null;
                    bb.Succeed();
                }
            }

            private bool ShouldLeaveExploration(State bb)
            {
                return bb.Hero.Stamina.ExhaustionPercent > 0.5f || bb.Hero.Health.Percent < 0.2f;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Behaviour ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private Behaviour m_behaviour;

        public Explore()
        {
            m_behaviour = new Behaviour(this);
        }

        public override bool CanHeroDoBehaviour(Hero hero)
        {
            return hero.Stamina.ExhaustionPercent < 0.5f || hero.Health.Percent > 0.2f;
        }

        public override float GetPriority(Hero hero)
        {
            return 1.0f;
        }

        protected override IHeroBehaviour.IBehaviour InitializeBehaviour()
        {
            m_behaviour.Initialize();

            return m_behaviour;
        }
    }
}
