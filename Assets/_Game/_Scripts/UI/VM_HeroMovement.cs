using SegoLib;
using SegoLib.UI.MVVM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VM_HeroMovement : VM_Base<HeroMovement>
    {
        public IRouteTraversal CurrentRoute
        {
            get => m_currentRoute;
            set => PropertySetter.Set(ref m_currentRoute, value, value == m_currentRoute, OnPropertyChanged);
        }
        private IRouteTraversal m_currentRoute;

        public float MoveSpeed
        {
            get => m_moveSpeed;
            set => PropertySetter.Set(ref m_moveSpeed, value, value == m_moveSpeed, OnPropertyChanged);
        }
        private float m_moveSpeed;

        public float RouteProgress
        {
            get => m_routeProgress;
            private set => PropertySetter.Set(ref m_routeProgress, value, value == m_routeProgress, OnPropertyChanged);
        }
        private float m_routeProgress;

        public ILocation Location
        {
            get => m_location;
            private set => PropertySetter.Set(ref m_location, value, value == m_location, OnPropertyChanged);
        }
        private ILocation m_location;

        protected override bool IsEqual(HeroMovement a, HeroMovement b) => a == b;

        protected override void SetValuesToDefault()
        {
            CurrentRoute = null;
            MoveSpeed = 0;
            RouteProgress = 0;
            Location = null;
        }

        protected override void UpdateValues()
        {
            CurrentRoute = Model.CurrentRoute;
            MoveSpeed = Model.MoveSpeed;
            RouteProgress = Model.RouteProgress;
            Location = Model.Location;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.currentRouteChanged += Model_CurrentRouteChanged;
            Model.moveSpeedChanged += Model_MoveSpeedChanged;
            Model.routeProgressChanged += Model_RouteProgressChanged;
            Model.locationChanged += Model_LocationChanged;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.currentRouteChanged -= Model_CurrentRouteChanged;
            Model.moveSpeedChanged -= Model_MoveSpeedChanged;
            Model.routeProgressChanged -= Model_RouteProgressChanged;
            Model.locationChanged -= Model_LocationChanged;
        }

        private void Model_LocationChanged(HeroMovement source, ILocation oldValue, ILocation newValue)
        {
            Location = newValue;
        }

        private void Model_RouteProgressChanged(HeroMovement source, float oldValue, float newValue)
        {
            RouteProgress = newValue;
        }

        private void Model_MoveSpeedChanged(HeroMovement source, float oldValue, float newValue)
        {
            MoveSpeed = newValue;
        }

        private void Model_CurrentRouteChanged(HeroMovement source, IRouteTraversal oldValue, IRouteTraversal newValue)
        {
            CurrentRoute = newValue;
        }
    } 
}
