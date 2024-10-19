using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroBrain : HeroComponent
    {
        public bool Initialized { get; private set; }

        private Rest m_restAtCamp = new Rest() { RestQuality = RestQuality.Terrible };

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
            m_currentState = null;
            stateChanged = null;
            m_availableBehaviours.Clear();
        }

        private void SetBehaviour(IHeroBehaviour newBehaviour)
        {
            if (newBehaviour == null) 
                return;

            if (m_currentState != null && m_currentState.Behaviour == newBehaviour)
                return;

            IHeroBehaviour.IState oldState = m_currentState;

            if(oldState != null && !oldState.Status.IsConcludedSatus()) {
                m_currentState.stateConcluded -= OnBehaviourConcluded;
                oldState.Cancel();
                m_currentState = null;
            }

            m_currentState = newBehaviour.BeginBehaviour(Hero);

            stateChanged?.Invoke(this, oldState, m_currentState);
        }

        private void OnBehaviourConcluded(IHeroBehaviour.IState state, IHeroBehaviour.EResult result)
        {
            throw new System.NotImplementedException();
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

            // TODO: SET DEFAULT BEHAVIOUR HERE
            if (availableBehaviours.Count == 0)
                return null;

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
