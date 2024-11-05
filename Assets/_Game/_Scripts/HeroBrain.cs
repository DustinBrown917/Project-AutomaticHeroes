using Game.AI.General;
using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroBrain : HeroComponent
    {
        public IHeroBehaviour.IState CurrentState
        {
            get => m_currentState;
        }
        private IHeroBehaviour.IState m_currentState;
        public event ValueChangedWithSourceEvent<HeroBrain, IHeroBehaviour.IState> stateChanged;

        public BasePriorities BasePriorities { get; } = new BasePriorities(0.98f, 0.3f, 1.0f, 0.5f, 0.5f);

        private readonly HashSet<IHeroBehaviour> m_availableBehaviours = new();

        [SerializeField] private HeroBehaviourGraph m_campGraph;
        [SerializeField] private HeroBehaviourGraph m_waitGraph;

        protected override void OnInitialize()
        {
            AddBehaviour(m_campGraph);

            SetBehaviour(GetRandomState());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(m_currentState != null) {
                m_currentState.StateConcluded -= OnBehaviourConcluded;
                m_currentState.Disposed -= CurrentStateDisposed;
                m_currentState.Cancel();
                m_currentState = null;
            }

            stateChanged = null;
            m_availableBehaviours.Clear();
        }

        private void SetBehaviour(IHeroBehaviour newBehaviour)
        {
            if (newBehaviour == null) 
                return;

            if (m_currentState != null && m_currentState.Behaviour == newBehaviour && !m_currentState.BehaviourStatus.IsConcludedSatus())
                return;

            IHeroBehaviour.IState oldState = m_currentState;

            if(oldState != null) {
                m_currentState.StateConcluded -= OnBehaviourConcluded;
                m_currentState.Disposed -= CurrentStateDisposed;

                if (!oldState.BehaviourStatus.IsConcludedSatus()) {
                    oldState.Cancel();
                }

                m_currentState = null;
            }

            m_currentState = newBehaviour.GetNewState(WorldState.Instance, Hero);
            m_currentState.StateConcluded += OnBehaviourConcluded;
            m_currentState.Disposed += CurrentStateDisposed;

            stateChanged?.Invoke(this, oldState, m_currentState);
        }

        private void CurrentStateDisposed(IPrivatelyDisposable obj)
        {
            Debug.LogError($"CurrentState was disposed before it was removed from HeroBrain. This shouldn't happen.");
            SetBehaviour(GetRandomState());
        }

        private void OnBehaviourConcluded(IHeroBehaviour.IState state, IHeroBehaviour.EResult result)
        {
            SetBehaviour(GetRandomState());
        }

        private IHeroBehaviour GetRandomState()
        {
            List<IHeroBehaviour> availableBehaviours = UnityEngine.Pool.ListPool<IHeroBehaviour>.Get();
            List<float> priorities = UnityEngine.Pool.ListPool<float>.Get();
            float sumOfPriorites = 0;

            foreach(IHeroBehaviour behaviour in m_availableBehaviours) {
                if (behaviour.CanHeroDoBehaviour(Hero)) {
                    availableBehaviours.Add(behaviour);
                    priorities.Add(behaviour.GetPriority(Hero));
                    sumOfPriorites += priorities[^1];
                }
            }

            if (availableBehaviours.Count == 0)
                return m_waitGraph;

            int index = WeightedRandom.Naive(priorities, sumOfPriorites, WorldState.Instance.Rng);

            IHeroBehaviour chosen = availableBehaviours[index];

            UnityEngine.Pool.ListPool<float>.Release(priorities);
            UnityEngine.Pool.ListPool<IHeroBehaviour>.Release(availableBehaviours);

            return chosen;
        }

        public void AddBehaviour(IHeroBehaviour behaviour)
        {
            m_availableBehaviours.Add(behaviour);
        }

        public void RemoveBehaviour(IHeroBehaviour behaviour)
        {
            m_availableBehaviours.Remove(behaviour);
        }
    }
}
