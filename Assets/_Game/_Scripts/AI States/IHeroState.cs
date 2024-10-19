using SegoLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial interface IHeroBehaviour
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Enums ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public enum EStatus
        {
            NotYetEntered,
            Active,
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

        public void Initialize();

        public IState BeginBehaviour(Hero hero);

        public void CancelState(int stateIndex);

        public void CancelAllStates();

        public float GetPriority(Hero hero);

        public bool CanHeroDoBehaviour(Hero hero);
    }

    public partial interface IHeroBehaviour
    {
        protected interface IBehaviour
        {
            /// <summary>
            /// Enter the state.
            /// </summary>
            /// <returns></returns>
            public IState BeginBehaviour(Hero hero);

            public void CancelState(int stateIndex);

            public void CancelAllStates();
        }
    }


    public abstract class HeroBehaviour : IHeroBehaviour
    {
        private IHeroBehaviour.IBehaviour m_behaviour;
        private bool m_initialized;

        public void Initialize()
        {
            if (m_initialized)
                return;

            m_behaviour = InitializeBehaviour();

            m_initialized = true;
        }

        public IHeroBehaviour.IState BeginBehaviour(Hero hero)
        {
            if (!m_initialized)
                Initialize();

            return m_behaviour.BeginBehaviour(hero);
        }
        public void CancelAllStates() => m_behaviour.CancelAllStates();
        public void CancelState(int stateIndex) => m_behaviour.CancelState(stateIndex);



        protected abstract IHeroBehaviour.IBehaviour InitializeBehaviour();
        public abstract bool CanHeroDoBehaviour(Hero hero);
        public abstract float GetPriority(Hero hero);
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

        public interface IState
        {
            public IHeroBehaviour Behaviour { get; }

            public int Index { get; }

            public Hero Hero { get; }

            public float ElapsedTime { get; }

            public EStatus Status { get; }
            public event BehaviourConcludedDelegate stateConcluded;

            public Sprite Sprite { get; }
            public event ValueChangedWithSourceEvent<IState, Sprite> spriteChanged;
            public string FlavourString { get; }
            public event ValueChangedWithSourceEvent<IState, string> flavourStringChanged;

            public void Cancel();
        }

        protected interface IState_Internal : IState, SegoLib.IDisposable
        {
            public void Succeed();
            public void Fail();
            public void UpdateIndex(int newIndex);
            public void AdvanceTime(float deltaSeconds);
        }
    }













    public partial interface IHeroBehaviour
    {
        protected class State : IState_Internal
        {
            public IHeroBehaviour Behaviour { get; }
            public int Index { get; private set; }

            public Hero Hero { get; private set; }
            public WorldState WorldState { get; private set; }

            public EStatus Status { get; private set; } = EStatus.NotYetEntered;
            public event BehaviourConcludedDelegate stateConcluded;

            public Sprite Sprite
            {
                get => m_sprite;
                protected set => SetSprite(value);
            }
            private Sprite m_sprite;
            public event ValueChangedWithSourceEvent<IState, Sprite> spriteChanged;

            public string FlavourString
            {
                get => m_flavourString;
                protected set => SetFlavourString(value);
            }
            private string m_flavourString;
            public event ValueChangedWithSourceEvent<IState, string> flavourStringChanged;

            public float ElapsedTime { get; private set; }

            public bool IsDisposed { get; private set; }
            public event Action<SegoLib.IDisposable> disposed;

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero)
            {
                Behaviour = behaviour;
                Hero = hero;
                WorldState = worldState;
            }

            #region SETTERS
            protected void SetSprite(Sprite newValue)
            {
                if (newValue == Sprite)
                    return;

                Sprite oldValue = m_sprite;
                m_sprite = newValue;

                spriteChanged?.Invoke(this, oldValue, newValue);
            }

            protected void SetFlavourString(string newValue)
            {
                if (newValue == null)
                    newValue = string.Empty;

                if (newValue == m_flavourString)
                    return;

                string oldValue = m_flavourString;
                m_flavourString = newValue;

                flavourStringChanged?.Invoke(this, oldValue, newValue);
            } 

            public void UpdateIndex(int newIndex)
            {
                Index = newIndex;
            }

            public void AdvanceTime(float deltaSeconds)
            {
                ElapsedTime += deltaSeconds;
            }
            #endregion /SETTERS

            public void Succeed()
            {
                if (Status.IsConcludedSatus())
                    return;

                Status = EStatus.Succeeded;
                stateConcluded?.Invoke(this, EResult.Succeeded);
            }

            public void Fail()
            {
                if (Status.IsConcludedSatus())
                    return;

                Status = EStatus.Failed;
                stateConcluded?.Invoke(this, EResult.Failed);
            }

            public void Cancel()
            {
                if (Status.IsConcludedSatus())
                    return;

                Status = EStatus.Cancelled;
                stateConcluded?.Invoke(this, EResult.Cancelled);
            }

            public void Dispose()
            {
                if (IsDisposed)
                    return;

                OnDisposed();

                WorldState = null;
                Hero = null;
                m_sprite = null;
                m_flavourString = null;

                spriteChanged = null;
                flavourStringChanged = null;

                disposed?.Invoke(this);
                disposed = null;
            }

            protected virtual void OnDisposed() { }
        }
    }






    public partial interface IHeroBehaviour
    {
        protected class StateCollection<T> where T : IState_Internal
        {
            public T this[int index] => m_blackboards[index];

            private readonly List<T> m_blackboards = new();

            public int Count => m_blackboards.Count;

            public void Add(T item)
            {
                m_blackboards.Add(item);
                item.UpdateIndex(Count - 1);
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index >= m_blackboards.Count)
                    return;

                m_blackboards.RemoveAt(index);

                UpdateIndeces(index);
            }

            private void UpdateIndeces(int fromIndex = 0)
            {
                for (int i = fromIndex; i < m_blackboards.Count; i++) {
                    m_blackboards[i].UpdateIndex(i);
                }
            }
        }
    }


    public partial interface IHeroBehaviour
    {
        protected abstract class Behaviour<T> : IBehaviour, WorldTime.IListener where T : IState_Internal
        {
            private StateCollection<T> m_blackboardCollection = new();

            public IHeroBehaviour ParentBehaviour { get; }

            public Behaviour(IHeroBehaviour parentBehaviour)
            {
                ParentBehaviour = parentBehaviour;
            }

            public void CancelState(int blackboardIndex)
            {
                T blackboard = m_blackboardCollection[blackboardIndex];
                blackboard.Cancel();
            }

            public void CancelAllStates()
            {
                for (int i = 0; i < m_blackboardCollection.Count; i++)
                    m_blackboardCollection[i].Cancel();
            }

            public void OnWorldTimeUpdate(float deltaSeconds)
            {
                for(int i = m_blackboardCollection.Count -1; i >= 0; i--) {

                    T blackboard = m_blackboardCollection[i];

                    if (blackboard.Status.IsConcludedSatus()) {
                        RetireBlackboard(blackboard);
                    } else {
                        blackboard.AdvanceTime(deltaSeconds);
                        OnWorldTimeUpdate(m_blackboardCollection[i], deltaSeconds);
                    }
                }
            }

            private void RetireBlackboard(T blackboard)
            {
                m_blackboardCollection.RemoveAt(blackboard.Index);
                blackboard.Dispose();

                if(m_blackboardCollection.Count == 0) {
                    if(WorldState.Instance != null && WorldState.Instance.Time != null) {
                        WorldState.Instance.Time.RemoveListener(this);
                    }
                }
            }


            public IState BeginBehaviour(Hero hero)
            {
                T blackboard = CreateNewState(hero);
                m_blackboardCollection.Add(blackboard);

                if (m_blackboardCollection.Count == 1)
                    WorldState.Instance.Time.AddListener(this);

                OnBegin(blackboard);
                return blackboard;
            }



            protected virtual void OnBegin(T blackboard) { }
            protected virtual void OnWorldTimeUpdate(T bb, float deltaSeconds) { }
            
            
            protected abstract T CreateNewState(Hero hero);
        }
    }
}
