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

        private readonly HashSet<IHeroBehaviourStateCreator> m_availableBehaviours = new();

        [SerializeField] private RestStateCreator m_campStateCreator;
        [SerializeField] private HeroTravelStateCreator m_travelStateCreator;
        [SerializeField] private HeroBehaviourStateCreator m_waitStateCreator;

        protected override void OnInitialize()
        {
            AddBehaviourStateCreator(m_campStateCreator);
            AddBehaviourStateCreator(m_travelStateCreator);

            SetBehaviour(m_waitStateCreator);
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

            m_campStateCreator = null;
            m_waitStateCreator = null;

            stateChanged = null;
            m_availableBehaviours.Clear();
        }

        private void SetBehaviour(IHeroBehaviourStateCreator newBehaviour)
        {
            if (newBehaviour == null) 
                return;

            if (m_currentState != null && m_currentState.Behaviour == newBehaviour.Behaviour && !m_currentState.BehaviourStatus.IsConcludedSatus())
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

            m_currentState = newBehaviour.CreateState(WorldState.Instance, Hero);
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

        private IHeroBehaviourStateCreator GetRandomState()
        {
            List<IHeroBehaviourStateCreator> availableBehaviours = UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Get();
            List<float> priorities = UnityEngine.Pool.ListPool<float>.Get();
            float sumOfPriorites = 0;

            foreach(IHeroBehaviourStateCreator behaviour in m_availableBehaviours) {
                if (behaviour.CanDo(WorldState.Instance, Hero)) {
                    availableBehaviours.Add(behaviour);
                    priorities.Add(behaviour.Utility(WorldState.Instance, Hero));
                    sumOfPriorites += priorities[^1];
                }
            }

            if (availableBehaviours.Count == 0) {
                ReleasePooledLists();
                return m_waitStateCreator;
            }
                

            int index = WeightedRandom.Naive(priorities, sumOfPriorites, WorldState.Instance.Rng);

            IHeroBehaviourStateCreator chosen = availableBehaviours[index];

            ReleasePooledLists();

            return chosen;

            void ReleasePooledLists()
            {
                UnityEngine.Pool.ListPool<float>.Release(priorities);
                UnityEngine.Pool.ListPool<IHeroBehaviourStateCreator>.Release(availableBehaviours);
            }
        }

        public void AddBehaviourStateCreator(IHeroBehaviourStateCreator behaviour)
        {
            m_availableBehaviours.Add(behaviour);
        }

        public void RemoveBehaviourStateCreator(IHeroBehaviourStateCreator behaviour)
        {
            m_availableBehaviours.Remove(behaviour);
        }
    }
}
