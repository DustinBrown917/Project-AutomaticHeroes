using SegoLib;
using SegoLib.Random;
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
        private IHeroState m_currentState = new State_Empty();
        public event ValueChangedWithSourceEvent<HeroBrain, IHeroState> stateChanged;


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

            oldValue.ExitState();
            oldValue.stateCompleted -= CurrentStateCompleted;

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
            if (WorldState.Instance.Rng.Bool()) {
                return new State_TravelToRandomNeighbour(Hero);
            } else {
                return new State_DoLocationAction(Hero);
            }
        }
    }

    public interface IStateProvider
    {

    }

    public interface IHeroState
    {
        public delegate void StateCompleteDelegate(IHeroState state);

        public Hero Hero { get; }

        public float Progress { get; }
        public event ValueChangedWithSourceEvent<IHeroState, float> progressChanged;

        public bool StateIsComplete { get; }
        public event StateCompleteDelegate stateCompleted;

        public void EnterState();

        public void DoState(float deltaSeconds);

        public void ExitState();

        public string GetActionString();
    }

    public class State_Empty : IHeroState
    {
        public Hero Hero => null;

        public float Progress => 0;

        public bool StateIsComplete => false;

#pragma warning disable CS0067
        public event ValueChangedWithSourceEvent<IHeroState, float> progressChanged;
        public event IHeroState.StateCompleteDelegate stateCompleted;
#pragma warning restore CS0067

        public void DoState(float deltaSeconds)
        {
            
        }

        public void EnterState()
        {
            
        }

        public void ExitState()
        {
            
        }

        public string GetActionString() => $"Doing nothing.";
    }

    public class State_DoLocationAction : IHeroState
    {
        public Hero Hero { get; }

        public float Progress
        {
            get => m_progress;
            private set => SetProgress(value);
        }
        private float m_progress;
        public event ValueChangedWithSourceEvent<IHeroState, float> progressChanged;

        public bool StateIsComplete { get; private set; }

        private float m_elapsedSeconds;
        private float m_secondsToComplete = 4;

        public event IHeroState.StateCompleteDelegate stateCompleted;

        public State_DoLocationAction(Hero hero)
        {
            Hero = hero;
        }

        public void EnterState()
        {
            
        }

        public void DoState(float deltaSeconds)
        {
            m_elapsedSeconds += deltaSeconds;
            SetProgress(m_elapsedSeconds / m_secondsToComplete);

            if(m_elapsedSeconds >= m_secondsToComplete) {
                CompleteState();
            }
        }

        public void ExitState()
        {
            
        }

        private void CompleteState()
        {
            if (StateIsComplete) return;

            StateIsComplete = true;
            stateCompleted?.Invoke(this);
        }

        private void SetProgress(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);

            if (m_progress == newValue) return;

            float oldValue = m_progress;
            m_progress = newValue;

            progressChanged?.Invoke(this, oldValue, newValue);

        }

        public string GetActionString() => $"Doing a thing.";
    }

    public class State_TravelToRandomNeighbour : IHeroState
    {
        public Hero Hero { get; }

        public float Progress
        {
            get => m_progress;
            private set => SetProgress(value);
        }
        private float m_progress;
        public event ValueChangedWithSourceEvent<IHeroState, float> progressChanged;

        public bool StateIsComplete { get; private set; }

        public event IHeroState.StateCompleteDelegate stateCompleted;

        public State_TravelToRandomNeighbour(Hero hero)
        {
            Hero = hero;

            Hero.Movement.CurrentRoute = GetRandomRouteToNeighbour(Hero.Movement.Location, WorldState.Instance.Rng);
            Hero.Movement.routeCompleted += Movement_RouteCompleted;
        }

        private void Movement_RouteCompleted(HeroMovement heroMovement, IRouteTraversal route)
        {
            Hero.Movement.routeCompleted -= Movement_RouteCompleted;
            CompleteState();
        }

        public void EnterState()
        {
            Hero.Movement.BeginMovement();
        }

        public void DoState(float deltaSeconds)
        {
            Hero.Movement.ProgressBySeconds(deltaSeconds);
            Progress = Hero.Movement.RouteProgress;
        }

        public void ExitState()
        {
            Hero.Movement.routeCompleted -= Movement_RouteCompleted;
        }

        private IRouteTraversal GetRandomRouteToNeighbour(ILocation location, IRandomNumberGenerator rng)
        {
            IRoute route = rng.GetRandom(location.Routes);

            IRouteTraversal routeTraversal;
            if (route.A == location) {
                routeTraversal = new RouteTraversalAToB(route);
            } else {
                routeTraversal = new RouteTraversalBToA(route);
            }

            return routeTraversal;
        }

        private void CompleteState()
        {
            if (StateIsComplete) return;

            StateIsComplete = true;
            stateCompleted?.Invoke(this);
        }

        private void SetProgress(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);

            if (m_progress == newValue) return;

            float oldValue = m_progress;
            m_progress = newValue;

            progressChanged?.Invoke(this, oldValue, newValue);

        }

        public string GetActionString() => $"Travelling to {Hero.Movement.CurrentRoute.End.DisplayName}.";
    }
}
