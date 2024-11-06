using Game.AI.General;
using Game.Flow;
using SegoLib;
using System;
using System.Collections.Generic;

namespace Game
{
    public partial interface IHeroBehaviour: IInitializable, WorldTime.IListener
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Enums ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public enum EStatus
        {
            NotYetEntered,
            Active,
            Paused,
            Succeeded,
            Failed,
            Cancelled
        }

        public enum EResult
        {
            Succeeded,
            Failed,
            Cancelled
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Enums ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Delegates ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public delegate void BehaviourConcludedDelegate(IState state, EResult result);

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Delegates ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        public IState GetNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData);

        public void CancelState(int stateIndex);

        public void CancelAllStates();

        public bool CanHeroDoBehaviour(Hero hero);
    }

    public static class EStatusExtensions
    {
        public static bool IsConcludedSatus(this IHeroBehaviour.EStatus status)
        {
            return status == IHeroBehaviour.EStatus.Succeeded ||
                status == IHeroBehaviour.EStatus.Failed ||
                status == IHeroBehaviour.EStatus.Cancelled;
        }
    }

    public partial interface IHeroBehaviour
    {
        public interface IStateManager : WorldTime.IListener
        {
            /// <summary>
            /// Enter the state.
            /// </summary>
            /// <returns></returns>
            public IState GetNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData);

            public void CancelState(int stateIndex);

            public void CancelAllStates();
        }

        /// <summary>
        /// Manages states associated with a behaviour.
        /// </summary>
        protected interface IStateManager<TBehaviour, TState> : IStateManager, WorldTime.IListener
            where TBehaviour : IHeroBehaviour
            where TState : IState_Internal
        {
            /// <summary>
            /// Enter the state.
            /// </summary>
            /// <returns></returns>
            public new TState GetNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData);
        }
    }





    public partial interface IHeroBehaviour
    {
        protected abstract class StateManager<TBehaviour, TState> : IStateManager<TBehaviour, TState>
            where TBehaviour : IHeroBehaviour 
            where TState : IState_Internal
        {
            private StateCollection<TState> m_stateCollection = new();

            public TBehaviour ParentBehaviour { get; }

            public StateManager(TBehaviour parentBehaviour)
            {
                ParentBehaviour = parentBehaviour;
            }

            public void CancelState(int stateIndex)
            {
                TState blackboard = m_stateCollection[stateIndex];
                blackboard.Cancel();
            }

            public void CancelAllStates()
            {
                for (int i = 0; i < m_stateCollection.Count; i++)
                    m_stateCollection[i].Cancel();
            }

            public void OnWorldTimeUpdate(float deltaSeconds)
            {
                for(int i = m_stateCollection.Count -1; i >= 0; i--) {

                    TState state = m_stateCollection[i];

                    if (state.BehaviourStatus.IsConcludedSatus()) {
                        RetireState(state);
                    } else if (state.BehaviourStatus == EStatus.Paused) {
                        continue;
                    } else {
                        state.OnWorldTimeUpdate(deltaSeconds);
                    }
                }
            }

            private void RetireState(TState state)
            {
                m_stateCollection.RemoveAt(state.Index);
                state.Dispose();
            }


            public TState GetNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData)
            {
                TState state = CreateNewState(worldState, hero, injectedData);
                m_stateCollection.Add(state);

                return state;
            }

            IState IStateManager.GetNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData) => GetNewState(worldState, hero, injectedData);
            
            protected abstract TState CreateNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData);
        }
    }


    public partial interface IHeroBehaviour
    {
        public interface INode
        {
            public interface IState : IPrivatelyDisposable
            {
                public bool IsConcluded { get; }
                public INode NextNode { get; }
                public void Conclude(INode nextNode);
            }
            protected interface IState_Internal : IState, SegoLib.IDisposable
            {
            }

            protected class State : IState_Internal
            {
                public bool IsConcluded { get; private set; }
                public INode NextNode { get; private set; }

                public bool IsDisposed { get; private set; }
                public event Action<IPrivatelyDisposable> Disposed;

                /// <summary>
                /// Disposed in IHeroBehaviour.Node when the node is exited.
                /// </summary>
                public void Dispose()
                {
                    if(IsDisposed) return;
                    IsDisposed = true;

                    OnDisposed();

                    Disposed?.Invoke(this);
                    Disposed = null;
                }

                protected virtual void OnDisposed() { }

                public void Conclude(INode nextNode)
                {
                    IsConcluded = true;
                    NextNode = nextNode;
                }
            }

            public IState Enter(IHeroBehaviour.IState bb);
            public void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IState node_bb, float deltaSeconds);
            public void Exit(IState node_bb);
        }

        [NodeWidth(300)]
        public abstract class Node : FlowNode, INode
        {
            public abstract INode.IState Enter(IState bb);

            public void Exit(INode.IState node_bb)
            {
                if(node_bb is INode.IState_Internal state_internal) {
                    state_internal.Dispose();
                }
            }

            public abstract void OnWorldTimeUpdate(IState bb, INode.IState node_bb, float deltaSeconds);

            public void ConcludeViaPort(INode.IState node_bb, FlowPort port)
            {
                node_bb.Conclude(port.ConnectedPortData?.Node as INode);
            }
        }
    }


    public interface IHeroBehaviourStateCreator
    {
        public IHeroBehaviour Behaviour { get; }

        public IHeroBehaviour.IState CreateState(WorldState worldState, Hero hero);

        public float Utility(WorldState worldState, Hero hero);
        public bool CanDo(WorldState worldState, Hero hero);
    }
}
