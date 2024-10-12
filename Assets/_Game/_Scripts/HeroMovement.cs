using SegoLib;
using UnityEngine;

namespace Game
{
    public class HeroMovement : HeroComponent
    {
        public delegate void RouteCompletedDelegate(HeroMovement heroMovement, IRouteTraversal route);

        public IRouteTraversal CurrentRoute
        {
            get => m_currentRoute;
            set => SetCurrentRoute(value);
        }
        private IRouteTraversal m_currentRoute;
        public event ValueChangedWithSourceEvent<HeroMovement, IRouteTraversal> currentRouteChanged;

        public float MoveSpeed
        {
            get => m_moveSpeed;
            set => SetMoveSpeed(value);
        }
        private float m_moveSpeed = 4;
        public event ValueChangedWithSourceEvent<HeroMovement, float> moveSpeedChanged;

        public float RouteProgress
        {
            get => m_routeProgress;
            private set => SetRouteProgress(value);
        }
        private float m_routeProgress;
        public event ValueChangedWithSourceEvent<HeroMovement, float> routeProgressChanged;

        public ILocation Location
        {
            get => m_location;
            private set => SetLocation(value);
        }
        private ILocation m_location;
        public event ValueChangedWithSourceEvent<HeroMovement, ILocation> locationChanged;

        private float m_movePerSecond = 0;

        private float m_routeDistanceTravelled;

        public event RouteCompletedDelegate routeCompleted;

        public bool Initialized { get; private set; }


        private void OnEnable()
        {
            if (m_currentRoute == null || IsTraversalComplete())
                enabled = false;
        }

        public void Initialize(ILocation location)
        {
            if (Initialized)
                return;

            transform.position = location.Position;
            Location = location;

            UpdateMovePerSecond();

            Initialized = true;
        }

        public void BeginMovement()
        {
            enabled = m_currentRoute != null;

            if (enabled && Location != null)
                Location = null;
        }

        public void StopMovement()
        {
            enabled = false;
        }

        public void ProgressBySeconds(float deltaTime)
        {
            if (!enabled)
                return;

            m_routeDistanceTravelled += deltaTime * m_movePerSecond;
            RouteProgress = m_routeDistanceTravelled / m_currentRoute.Length;
            UpdatePosition();

            if (IsTraversalComplete()) {
                StopMovement();
                Location = CurrentRoute.End;
                routeCompleted?.Invoke(this, CurrentRoute);
            }
        }

        public void SetMoveSpeed(float newSpeed)
        {
            if (m_moveSpeed == newSpeed)
                return;

            if (newSpeed < 0)
                newSpeed = 0;

            float oldSpeed = m_moveSpeed;
            m_moveSpeed = newSpeed;
            UpdateMovePerSecond();
            moveSpeedChanged?.Invoke(this, oldSpeed, newSpeed);
        }

        private void SetRouteProgress(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);

            if (newValue == m_routeProgress) return;
            
            float oldValue = m_routeProgress;
            m_routeProgress = newValue;

            routeProgressChanged?.Invoke(this, oldValue, newValue);
        }

        private void SetLocation(ILocation newValue)
        {
            if (m_location == newValue) return;

            ILocation oldValue = m_location;
            m_location = newValue;

            if (oldValue != null)
                oldValue.OnLocationExit(Hero);

            if(newValue != null)
                newValue.OnLocationEnter(Hero);

            locationChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetCurrentRoute(IRouteTraversal newRoute)
        {
            if(newRoute == m_currentRoute) return;

            IRouteTraversal oldRoute = m_currentRoute;
            m_currentRoute = newRoute;

            if(m_currentRoute == null) {
                StopMovement();
            }

            RouteProgress = 0;
            m_routeDistanceTravelled = 0;

            if(newRoute != null) {
                UpdatePosition();
            }

            currentRouteChanged?.Invoke(this, oldRoute, newRoute);
        }

        private void UpdatePosition()
        {
            transform.position = m_currentRoute.Traverse(RouteProgress);
        }

        private bool IsTraversalComplete()
        {
            return m_routeDistanceTravelled >= CurrentRoute.Length;
        }

        private void UpdateMovePerSecond()
        {
            m_movePerSecond = MoveSpeed / 3600;
        }
    }

}