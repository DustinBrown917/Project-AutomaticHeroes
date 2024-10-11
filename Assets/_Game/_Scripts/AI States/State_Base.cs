using SegoLib;
using System;
using UnityEngine;

namespace Game
{
    public abstract class State_Base : IHeroState
    {
        public IStateFactory StateFactory { get; private set; }

        public Hero Hero { get; private set; }
        public float Progress
        {
            get => m_progress;
            protected set => SetProgress(value);
        }
        private float m_progress;
        public event ValueChangedWithSourceEvent<IHeroState, float> progressChanged;

        public bool StateIsComplete { get; private set; }
        public event IHeroState.StateCompleteDelegate stateCompleted;

        public bool StateIsCancelled { get; private set; }
        public event IHeroState.StateCancelledDelegate stateCancelled;

        public bool IsDisposed { get; private set; }
        public event Action<SegoLib.IDisposable> disposed;

        public State_Base(IStateFactory stateFactory, Hero hero)
        {
            StateFactory = stateFactory;
            Hero = hero;
        }

        public abstract void DoState(float deltaSeconds);

        public abstract void EnterState();

        public abstract void ExitState();

        public abstract string GetActionString();

        protected virtual void OnComplete() { }

        protected void CompleteState()
        {
            if(StateIsComplete) return;
            StateIsComplete = true;
            OnComplete();
            stateCompleted?.Invoke(this);
        }

        public void Cancel()
        {
            if (StateIsCancelled) return;
            StateIsCancelled = true;
            stateCancelled?.Invoke(this);
        }

        private void SetProgress(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);

            if (m_progress == newValue) return;

            float oldValue = m_progress;
            m_progress = newValue;

            progressChanged?.Invoke(this, oldValue, newValue);
        }

        public void Dispose()
        {
            if(IsDisposed) return;
            IsDisposed = true;
            OnDisposing();

            Hero = null;
            StateFactory = null;

            progressChanged = null;
            stateCompleted = null;
            stateCancelled = null;

            disposed?.Invoke(this);
            disposed = null;
        }

        protected virtual void OnDisposing() { }
    } 
}
