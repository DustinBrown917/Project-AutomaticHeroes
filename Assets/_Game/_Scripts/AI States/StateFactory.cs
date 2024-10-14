using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IStateFactory : SegoLib.IDisposable
    {
        public Sprite Sprite { get; }

        public bool CanCreateNewState { get; set; }

        public abstract IHeroState CreateState(Hero hero);

        public abstract void CancelCreatedStates();
    }

    public abstract class StateFactory<T> : IStateFactory where T : IHeroState
    {
        public abstract Sprite Sprite { get; }

        public bool CanCreateNewState { get; set; } = true;

        private readonly HashSet<T> m_createdStates = new();

        public bool IsDisposed { get; private set; }
        public event Action<SegoLib.IDisposable> disposed;

        public void CancelCreatedStates()
        {
            List<T> states = UnityEngine.Pool.ListPool<T>.Get();
            states.AddRange(m_createdStates);

            for(int i = 0; i < m_createdStates.Count; i++) {
                states[i].Cancel();
            }

            UnityEngine.Pool.ListPool<T>.Release(states);
        }

        IHeroState IStateFactory.CreateState(Hero hero) => CreateState(hero);

        public T CreateState(Hero hero)
        {
            T state = CreateState_Internal(hero);
            m_createdStates.Add(state);
            SubscribeToEvents(state);
            return state;
        }

        protected abstract T CreateState_Internal(Hero hero);

        private void SubscribeToEvents(T state)
        {
            state.stateCancelled += StateCancelled;
            state.stateCompleted += StateCompleted;
            state.disposed += StateDisposed;
        }

        private void UnsubscribeEvents(T state)
        {
            state.stateCancelled -= StateCancelled;
            state.stateCompleted -= StateCancelled;
            state.disposed -= StateDisposed;
        }

        private void RemoveStateFromTracking(T state)
        {
            m_createdStates.Remove(state);
            UnsubscribeEvents(state);
        }

        private void StateCompleted(IHeroState state)
        {
            RemoveStateFromTracking((T)state);
        }

        private void StateCancelled(IHeroState state)
        {
            RemoveStateFromTracking((T)state);
        }

        private void StateDisposed(SegoLib.IDisposable obj)
        {
            RemoveStateFromTracking((T)obj);
        }

        public void Dispose()
        {
            if(IsDisposed) return;
            IsDisposed = true;

            UnsubAllStates();
            m_createdStates.Clear();

            disposed?.Invoke(this);
            disposed = null;
        }

        private void UnsubAllStates()
        {
            foreach(T state in m_createdStates) {
                UnsubscribeEvents(state);
            }
        }
    }
}
