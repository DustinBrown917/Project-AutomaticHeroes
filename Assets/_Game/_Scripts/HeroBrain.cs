using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Game
{
    public class HeroBrain : HeroComponent
    {
        public static Camp Camp { get; } = new Camp();

        public bool Initialized { get; private set; }

        public IHeroState CurrentState
        {
            get => m_currentState;
            set => SetState(value);
        }
        private IHeroState m_currentState;
        public event ValueChangedWithSourceEvent<HeroBrain, IHeroState> stateChanged;

        public BasePriorities BasePriorities { get; } = new BasePriorities(0.98f, 0.3f, 1.0f, 0.5f, 0.5f);

        private readonly HashSet<IStateFactory> m_availableStates = new();

        public void Initialize()
        {
            if (Initialized)
                return;

            AddStateFactory(Camp);

            CurrentState = GetRandomState();

            Initialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_currentState = null;
            stateChanged = null;
            m_availableStates.Clear();
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

            StringBuilder sb = new StringBuilder();
            sb.Append("Time of day: ");
            sb.Append(WorldState.Instance.Date.TimeOfDay.ToString());
            sb.Append(" || Hero Stamina: ");
            sb.Append(Hero.Stamina.Percent);
            sb.Append(" || Hero Health: ");
            sb.Append(Hero.Health.Percent);
            sb.Append(" || Hero Treated Wounds: ");
            sb.Append(Hero.Health.TreatedPercent);
            sb.Append(" || Hero Location: ");
            sb.Append(Hero.Movement.Location.DisplayName);
            sb.AppendLine();
            for (int i = 0; i < availableStates.Count; i++) {
                sb.Append(availableStates[i].GetType().Name);
                sb.Append(" = ");
                sb.Append(priorities[i]);
                sb.AppendLine();
            }
            UnityEngine.Debug.Log(sb.ToString());

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
