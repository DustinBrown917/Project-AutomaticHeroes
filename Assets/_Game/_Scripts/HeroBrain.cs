using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;

namespace Game
{
    public class HeroBrain : HeroComponent, WorldTime.IListener
    {
        public bool Initialized { get; private set; }

        public IHeroState CurrentState
        {
            get => m_currentState;
            set => SetState(value);
        }
        private IHeroState m_currentState;
        public event ValueChangedWithSourceEvent<HeroBrain, IHeroState> stateChanged;

        private readonly HashSet<IStateFactory> m_availableStates = new();

        public void Initialize()
        {
            if (Initialized)
                return;

            CurrentState = GetRandomState();

            WorldState.Instance.Time.AddListener(this);

            Initialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(WorldState.Instance != null && WorldState.Instance.Time != null) {
                WorldState.Instance.Time.RemoveListener(this);
            }
        }

        public void OnWorldTimeUpdate(float deltaSeconds)
        {
            if (CurrentState != null) {
                CurrentState.DoState(deltaSeconds);
            } else {
                CurrentState = GetRandomState();
            }
        }

        private void SetState(IHeroState newValue)
        {
            if (newValue == null || newValue == CurrentState) 
                return;

            IHeroState oldValue = m_currentState;
            m_currentState = newValue;

            if(oldValue != null) {
                oldValue.ExitState();
                oldValue.stateCompleted -= CurrentStateCompleted;
            }


            m_currentState.stateCompleted += CurrentStateCompleted;
            m_currentState.EnterState();

            stateChanged?.Invoke(this, oldValue, newValue);
        }

        private void CurrentStateCompleted(IHeroState state)
        {
            CurrentState = GetRandomState();
        }

        private IHeroState GetRandomState()
        {
            List<IStateFactory> availableStates = UnityEngine.Pool.ListPool<IStateFactory>.Get();
            List<float> priorities = UnityEngine.Pool.ListPool<float>.Get();
            float sumOfPriorites = 0;

            foreach(IStateFactory factory in m_availableStates) {
                if (factory.CanCreateNewState) {
                    availableStates.Add(factory);
                    priorities.Add(factory.GetPriority(Hero));
                    sumOfPriorites += priorities[^1];
                }

            }

            if (availableStates.Count == 0)
                return WorldState.Instance.DefaultWaitStateFactory.CreateState(Hero);

            int index = WeightedRandom.Naive(priorities, sumOfPriorites, WorldState.Instance.Rng);

            IStateFactory chosen = availableStates[index];

            UnityEngine.Pool.ListPool<float>.Release(priorities);
            UnityEngine.Pool.ListPool<IStateFactory>.Release(availableStates);

            return chosen.CreateState(Hero);
        }

        public void AddStateFactory(IStateFactory factory)
        {
            m_availableStates.Add(factory);
        }

        public void RemoveStateFactory(IStateFactory factory)
        {
            m_availableStates.Remove(factory);
        }

    }






}
