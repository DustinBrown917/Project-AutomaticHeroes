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

            public IExploreNodeState CurrentExploreNodeState { get; set; }

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero) : base(behaviour, worldState, hero)
            {
            }

            protected override void OnDisposed()
            {
                CurrentExploreNodeState.Dispose();
                CurrentExploreNodeState = null;
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
            public ExploreGraph exploreGraph;
            public bool Initialized { get; private set; }

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour)
            {
            }

            public void Initialize()
            {
                exploreGraph.Initialize();
            }

            protected override void OnBegin(State bb)
            {
                ExploreNode entryNode = FindEntryNode(bb);
                EnterNode(bb, entryNode);
            }

            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                bb.CurrentExploreNodeState.Node.OnWorldTimeUpdate(bb, deltaSeconds);

                if (bb.CurrentExploreNodeState.IsResolved) {
                    if (bb.CurrentExploreNodeState.Node.IsExitNode) {
                        bb.Succeed();
                        return;
                    }

                    ExploreNode nextNode;

                    if (!ShouldLeaveExploration(bb)) {
                        nextNode = FindNextNode(bb);
                    } else {
                        nextNode = exploreGraph.DeadEndExitNode;
                    }

                    if (nextNode == null) {
                        nextNode = exploreGraph.DeadEndExitNode;
                    }

                    EnterNode(bb, nextNode);
                }
            }

            protected override State CreateNewState(Hero hero)
            {
                State newState = new(ParentBehaviour, WorldState.Instance, hero);
                return newState;
            }

            private ExploreNode FindEntryNode(State bb)
            {
                if(exploreGraph.EntryNodes.Count > 0) {
                    return bb.WorldState.Rng.GetRandom(exploreGraph.EntryNodes);
                } else {
                    return exploreGraph.ExploreNodes[0];
                }
            }

            private void EnterNode(State bb, ExploreNode node)
            {
                IExploreNodeState oldNodeState = bb.CurrentExploreNodeState;
                bb.CurrentExploreNodeState = node.Enter(bb);
                oldNodeState?.Dispose();
            }

            private ExploreNode FindNextNode(State bb)
            {
                List<ExplorePort> validExplorePorts = UnityEngine.Pool.ListPool<ExplorePort>.Get();
                List<NodePort> validNodePorts = UnityEngine.Pool.ListPool<NodePort>.Get();

                ExploreNode returnNode = null;

                NodeResolution resolution = bb.CurrentExploreNodeState.Resolution;

                foreach(NodePort nodePort in bb.CurrentExploreNodeState.Node.Outputs) {
                    if (!nodePort.IsConnected)
                        continue;

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

                    returnNode = validNodePorts[index].Connection.node as ExploreNode;
                }

                UnityEngine.Pool.ListPool<NodePort>.Release(validNodePorts);
                UnityEngine.Pool.ListPool<ExplorePort>.Release(validExplorePorts);

                return returnNode;
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
