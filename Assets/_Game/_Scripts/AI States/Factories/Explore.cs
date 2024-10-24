using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Exploration;
using XNode;
using SegoLib.Random;

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

            public IExploreNodeState CurrentExploreNodeState { get; set; }

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

            public bool Initialized { get; private set; }

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour)
            {
            }

            public void Initialize()
            {

            }

            protected override void OnBegin(State bb)
            {

            }

            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                if(bb.CurrentExploreNodeState.IsResolved) {
                    ExploreNode nextNode = FindNextNode(bb);

                    if (nextNode == null) {
                        bb.Succeed();
                        return;
                    }

                    bb.CurrentExploreNodeState = null;
                    bb.CurrentExploreNodeState = nextNode.Enter(bb);
                }

                bb.CurrentExploreNodeState.Node.OnWorldTimeUpdate(bb, deltaSeconds);
            }

            protected override State CreateNewState(Hero hero)
            {
                State newState = new(ParentBehaviour, WorldState.Instance, hero);
                return newState;
            }

            private ExploreNode FindNextNode(State bb)
            {
                List<ExplorePort> validExplorePorts = UnityEngine.Pool.ListPool<ExplorePort>.Get();
                List<NodePort> validNodePorts = UnityEngine.Pool.ListPool<NodePort>.Get();

                ExploreNode returnNode = null;

                NodeResolution resolution = bb.CurrentExploreNodeState.Resolution;

                foreach(NodePort nodePort in bb.CurrentExploreNodeState.Node.Outputs) {
                    ExplorePort explorePort = bb.CurrentExploreNodeState.Node.GetPort(nodePort);

                    if (explorePort.IsValidForResolution(resolution)) {
                        validExplorePorts.Add(explorePort);
                        validNodePorts.Add(nodePort);
                    }
                }

                if(validExplorePorts.Count > 0) {
                    float sumOfWeights = 0;
                    for(int i = 0; i < validExplorePorts.Count; i++) {
                        sumOfWeights += validExplorePorts[i].Weight;
                    }

                    int index = 0;
                    if(sumOfWeights == 0) {
                        index = bb.WorldState.Rng.Range(0, validNodePorts.Count);
                    } else {
                        index = WeightedRandom.Naive(validExplorePorts, sumOfWeights, bb.WorldState.Rng, x => x.Weight);
                    }

                    returnNode = validNodePorts[index].node as ExploreNode;
                }

                UnityEngine.Pool.ListPool<NodePort>.Release(validNodePorts);
                UnityEngine.Pool.ListPool<ExplorePort>.Release(validExplorePorts);

                return returnNode;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Behaviour ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private Behaviour m_behaviour;
        [SerializeField] private ExploreGraph m_exploreGraph;

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
            m_behaviour.Initialize();

            return m_behaviour;
        }
    }
}
