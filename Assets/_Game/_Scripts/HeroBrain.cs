using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroBrain : HeroComponent
    {
        public bool Initialized { get; private set; }

        private Rest m_restAtCamp = new Rest() { StateString = "Resting at camp.", RestQuality = RestQuality.Terrible, Sprite = SpriteLibrary.GetSprite("fire")};
        private Wait m_wait = new Wait() { StateString = "Waiting...", Sprite = SpriteLibrary.GetSprite("Wait Icon") };

        public IHeroBehaviour.IState CurrentState
        {
            get => m_currentState;
        }
        private IHeroBehaviour.IState m_currentState;
        public event ValueChangedWithSourceEvent<HeroBrain, IHeroBehaviour.IState> stateChanged;

        public BasePriorities BasePriorities { get; } = new BasePriorities(0.98f, 0.3f, 1.0f, 0.5f, 0.5f);

        private readonly HashSet<IHeroBehaviour> m_availableBehaviours = new();

        public void Initialize()
        {
            if (Initialized)
                return;

            AddBehaviour(m_restAtCamp);

            SetBehaviour(GetRandomState());

            Initialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(m_currentState != null) {
                m_currentState.stateConcluded -= OnBehaviourConcluded;
                m_currentState.disposed -= CurrentStateDisposed;
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

            if (m_currentState != null && m_currentState.Behaviour == newBehaviour && !m_currentState.Status.IsConcludedSatus())
                return;

            IHeroBehaviour.IState oldState = m_currentState;

            if(oldState != null) {
                m_currentState.stateConcluded -= OnBehaviourConcluded;
                m_currentState.disposed -= CurrentStateDisposed;

                if (!oldState.Status.IsConcludedSatus()) {
                    oldState.Cancel();
                }

                m_currentState = null;
            }

            m_currentState = newBehaviour.BeginBehaviour(Hero);
            m_currentState.stateConcluded += OnBehaviourConcluded;
            m_currentState.disposed += CurrentStateDisposed;

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

            //StringBuilder sb = new StringBuilder();
            //sb.Append("Time of day: ");
            //sb.Append(WorldState.Instance.Date.TimeOfDay.ToString());
            //sb.Append(" || Hero Stamina: ");
            //sb.Append(Hero.Stamina.EnergyPercent);
            //sb.Append(" || Hero Health: ");
            //sb.Append(Hero.Health.Percent);
            //sb.Append(" || Hero Treated Wounds: ");
            //sb.Append(Hero.Health.TreatedPercent);
            //sb.Append(" || Hero Location: ");
            //sb.Append(Hero.Movement.Location.DisplayName);
            //sb.AppendLine();
            //for (int i = 0; i < availableStates.Count; i++) {
            //    sb.Append(availableStates[i].GetType().Name);
            //    sb.Append(" = ");
            //    sb.Append(priorities[i]);
            //    sb.AppendLine();
            //}
            //UnityEngine.Debug.Log(sb.ToString());

            if (availableBehaviours.Count == 0)
                return m_wait;

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
