using Game.Flow;
using SegoLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial interface IHeroBehaviour
    {
        public interface IState : IPrivatelyDisposable, WorldTime.IListener
        {
            public IHeroBehaviour Behaviour { get; }

            public int Index { get; }
            public WorldState WorldState { get; }
            public Hero Hero { get; }

            public float ElapsedTime { get; }

            public EStatus BehaviourStatus { get; }
            public event BehaviourConcludedDelegate StateConcluded;

            public Sprite ForegroundSprite { get; }
            public event ValueChangedWithSourceEvent<IState, Sprite> ForegroundSpriteChanged;

            public Sprite BackgroundSprite { get; }
            public event ValueChangedWithSourceEvent<IState, Sprite> BackgroundSpriteChanged;

            public string FlavourText { get; set; }
            public event ValueChangedWithSourceEvent<IState, string> FlavourTextChanged;
            public string StateText { get; set; }
            public event ValueChangedWithSourceEvent<IState, string> StateTextChanged;

            public void Pause();
            public void Unpause();
            public void Cancel();


            public void InjectData(Identity key, object value);
            public bool TryGetInjectedData<T>(Identity key, out T value);
            public bool RemoveInjectedData(Identity key);
        }

        protected interface IState_Internal : IState, SegoLib.IDisposable
        {
            public void Succeed();
            public void Fail();
            public void UpdateIndex(int newIndex);
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

            private INode m_currentNode;
            private INode.IState m_currentNodeState;

            public EStatus BehaviourStatus { get; private set; } = EStatus.NotYetEntered;
            public event BehaviourConcludedDelegate StateConcluded;

            public Sprite ForegroundSprite
            {
                get => m_foregroundSprite;
                set => SetForegroundSprite(value);
            }
            private Sprite m_foregroundSprite;
            public event ValueChangedWithSourceEvent<IState, Sprite> ForegroundSpriteChanged;

            public Sprite BackgroundSprite
            {
                get => m_backgroundSprite;
                set => SetBackgroundSprite(value);
            }
            private Sprite m_backgroundSprite;
            public event ValueChangedWithSourceEvent<IState, Sprite> BackgroundSpriteChanged;

            public string StateText
            {
                get => m_stateText;
                set => SetStateText(value);
            }
            private string m_stateText;
            public event ValueChangedWithSourceEvent<IState, string> StateTextChanged;

            public string FlavourText
            {
                get => m_flavourString;
                set => SetFlavourText(value);
            }
            private string m_flavourString;
            public event ValueChangedWithSourceEvent<IState, string> FlavourTextChanged;

            public float ElapsedTime { get; private set; }

            private readonly Dictionary<Identity, object> m_injectedData = new();

            public bool IsDisposed { get; private set; }
            public event Action<IPrivatelyDisposable> Disposed;

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero, INode entryNode, params (Identity key, object value)[] injectedData)
            {
                Behaviour = behaviour;
                Hero = hero;
                WorldState = worldState;

                if(injectedData != null) {
                    foreach((Identity key, object value) in injectedData) {
                        InjectData(key, value);
                    }
                }

                EnterNode(entryNode);
            }

            #region SETTERS
            protected void SetForegroundSprite(Sprite newValue)
            {
                if (newValue == ForegroundSprite)
                    return;

                Sprite oldValue = m_foregroundSprite;
                m_foregroundSprite = newValue;

                ForegroundSpriteChanged?.Invoke(this, oldValue, newValue);
            }

            protected void SetBackgroundSprite(Sprite newValue)
            {
                if (newValue == BackgroundSprite)
                    return;

                Sprite oldValue = m_backgroundSprite;
                m_backgroundSprite = newValue;

                BackgroundSpriteChanged?.Invoke(this, oldValue, newValue);
            }
            protected void SetFlavourText(string newValue)
            {
                if (newValue == null)
                    newValue = string.Empty;

                newValue = string.Format(WorldState.HeroFormatProvider, newValue, Hero);

                if (newValue == m_flavourString)
                    return;

                string oldValue = m_flavourString;
                m_flavourString = newValue;

                FlavourTextChanged?.Invoke(this, oldValue, newValue);
            }

            protected void SetStateText(string newValue)
            {
                if (newValue == null)
                    newValue = string.Empty;

                if (newValue == m_stateText)
                    return;

                string oldValue = m_stateText;
                m_stateText = newValue;

                StateTextChanged?.Invoke(this, oldValue, newValue);
            }

            public void InjectData(Identity key, object value)
            {
                m_injectedData.Add(key, value);
            }

            public bool TryGetInjectedData<T>(Identity key, out T value)
            {
                if(m_injectedData.TryGetValue(key, out object obj) && obj is T typedObj) {
                    value = typedObj;
                    return true;
                }

                value = default;
                return false;
            }

            public bool RemoveInjectedData(Identity key)
            {
                return m_injectedData.Remove(key);
            }

            public void UpdateIndex(int newIndex)
            {
                Index = newIndex;
            }
            #endregion /SETTERS

            private void EnterNode(INode node)
            {
                m_currentNode = node;

                if(m_currentNode != null)
                    m_currentNodeState = node.Enter(this);
            }

            private void ExitCurrentNode()
            {
                if(m_currentNode != null) {
                    m_currentNode.Exit(m_currentNodeState);
                }

                m_currentNode = null;
                m_currentNodeState = null;
            }

            public void Succeed()
            {
                if (BehaviourStatus.IsConcludedSatus())
                    return;

                BehaviourStatus = EStatus.Succeeded;
                StateConcluded?.Invoke(this, EResult.Succeeded);
            }

            public void Fail()
            {
                if (BehaviourStatus.IsConcludedSatus())
                    return;

                BehaviourStatus = EStatus.Failed;
                StateConcluded?.Invoke(this, EResult.Failed);
            }

            public void Cancel()
            {
                if (BehaviourStatus.IsConcludedSatus())
                    return;

                BehaviourStatus = EStatus.Cancelled;
                StateConcluded?.Invoke(this, EResult.Cancelled);
            }

            public void Pause()
            {
                if (BehaviourStatus == EStatus.Active)
                    BehaviourStatus = EStatus.Paused;
            }

            public void Unpause()
            {
                if (BehaviourStatus == EStatus.Paused)
                    BehaviourStatus = EStatus.Active;
            }

            /// <summary>
            /// Raised by a StateManager managing this state that is subscribed to WorldTime.
            /// </summary>
            /// <param name="deltaSeconds"></param>
            public void OnWorldTimeUpdate(float deltaSeconds)
            {
                ElapsedTime += deltaSeconds;

                while (m_currentNode != null && m_currentNodeState.IsConcluded) {
                    INode nextNode = m_currentNodeState.NextNode;

                    ExitCurrentNode();
                    EnterNode(nextNode);
                }

                if(m_currentNode != null) {
                    m_currentNode.OnWorldTimeUpdate(this, m_currentNodeState, deltaSeconds);
                } else {
                    Succeed();
                }
            }

            /// <summary>
            /// Disposed when the state is retired from a StateManager.
            /// </summary>
            public void Dispose()
            {
                if (IsDisposed)
                    return;

                OnDisposed();

                WorldState = null;
                Hero = null;
                m_foregroundSprite = null;
                m_backgroundSprite = null;
                m_flavourString = null;
                ExitCurrentNode();

                ForegroundSpriteChanged = null;
                BackgroundSpriteChanged = null;
                FlavourTextChanged = null;
                StateTextChanged = null;
                m_injectedData.Clear();

                Disposed?.Invoke(this);
                Disposed = null;
            }

            protected virtual void OnDisposed() { }
        }
    }
}
