using Game.Causes;
using SegoLib;
using SegoLib.UI.MVVM;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VM_ILocation : VM_Base<ILocation>
    {
        public string DisplayName
        {
            get => m_displayName;
            private set => PropertySetter.Set(ref m_displayName, value, m_displayName == value, OnPropertyChanged);
        }
        private string m_displayName;

        public IEnumerable<Hero> Visitors
        {
            get => m_visitors;
            private set {
                m_visitors.Clear();

                if(value != null)
                    m_visitors.AddRange(value);

                OnPropertyChanged();
            }
        }
        private readonly List<Hero> m_visitors = new();

        public CauseFavourTracker CauseFavourTracker
        {
            get => m_causeFavourTracker;
            private set => PropertySetter.Set(ref m_causeFavourTracker, value, m_causeFavourTracker == value, OnPropertyChanged);
        }
        private CauseFavourTracker m_causeFavourTracker;

        public Color[] CauseColors
        {
            get => m_causeColors;
            private set => PropertySetter.Set(ref m_causeColors, value, m_causeColors == value, OnPropertyChanged);
        }
        private Color[] m_causeColors = Array.Empty<Color>();

        public float[] CauseProportions
        {
            get => m_causeProportions;
            private set => PropertySetter.Set(ref m_causeProportions, value, m_causeProportions == value, OnPropertyChanged);
        }
        private float[] m_causeProportions = Array.Empty<float>();

        protected override bool IsEqual(ILocation a, ILocation b) => a == b;

        protected override void Awake()
        {
            Color[] colors = new Color[CauseDef.All.Count];
            for (int i = 0; i < CauseDef.All.Count; i++) {
                colors[CauseDef.All[i].RuntimeIndex] = CauseDef.All[i].Color;
            }

            CauseColors = colors;

            float[] proportions = new float[CauseDef.All.Count];
            proportions[0] = 1.0f;

            CauseProportions = proportions;

            base.Awake();
        }

        protected override void SetValuesToDefault()
        {
            DisplayName = string.Empty;
            Visitors = null;
            CauseFavourTracker = null;
            SetProportionsToDefault();
        }

        protected override void UpdateValues()
        {
            DisplayName = Model.DisplayName;
            Visitors = Model.Visitors;
            CauseFavourTracker = Model.CauseFavourTracker;
            UpdateProportionsFromCauseFavourTracker();
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.visitorsUpdated += Model_VisitorsUpdated;
            Model.CauseFavourTracker.Updated += Model_CauseFavourTracker_Updated;
        }


        protected override void OnModelChangingFromNotNull()
        {
            Model.visitorsUpdated -= Model_VisitorsUpdated;
            Model.CauseFavourTracker.Updated -= Model_CauseFavourTracker_Updated;
        }

        private void Model_VisitorsUpdated(ILocation source, IReadOnlyList<Hero> newValue)
        {
            Visitors = newValue;
        }

        private void SetProportionsToDefault()
        {
            for(int i = 1; i < m_causeProportions.Length; i++) {
                m_causeProportions[i] = 0.0f;
            }

            m_causeProportions[0] = 1.0f;

            OnPropertyChanged(nameof(CauseProportions));
        }

        private void Model_CauseFavourTracker_Updated(CauseFavourTracker obj)
        {
            UpdateProportionsFromCauseFavourTracker();
        }

        private void UpdateProportionsFromCauseFavourTracker()
        {
            foreach(CauseDef cause in CauseDef.All) {
                m_causeProportions[cause.RuntimeIndex] = CauseFavourTracker.GetFavourPercent(cause);
            }

            OnPropertyChanged(nameof(CauseProportions));
        }
    }

}