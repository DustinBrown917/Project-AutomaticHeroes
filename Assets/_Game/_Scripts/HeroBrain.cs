using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroBrain : HeroComponent
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

            Initialized = true;
        }

        private void Update()
        {
            if(CurrentState != null) {
                CurrentState.DoState(Time.deltaTime);
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
            
            foreach(IStateFactory factory in m_availableStates) {
                if(factory.CanCreateNewState)
                    availableStates.Add(factory);
            }

            if (availableStates.Count == 0)
                return WorldState.Instance.DefaultWaitStateFactory.CreateState(Hero);

            IStateFactory chosen = WorldState.Instance.Rng.GetRandom(availableStates);
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
