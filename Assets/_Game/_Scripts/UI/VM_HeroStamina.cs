using SegoLib;
using SegoLib.UI.MVVM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VM_HeroStamina : VM_Base<HeroStamina>
    {
        public float Current
        {
            get => m_current;
            private set => PropertySetter.Set(ref m_current, value, m_current == value, OnPropertyChanged);
        }
        private float m_current;

        public float Max
        {
            get => m_max;
            private set => PropertySetter.Set(ref m_max, value, m_max == value, OnPropertyChanged);
        }
        private float m_max;

        public float Percent
        {
            get => m_percent;
            private set => PropertySetter.Set(ref m_percent, value, m_percent == value, OnPropertyChanged);
        }
        private float m_percent;

        protected override bool IsEqual(HeroStamina a, HeroStamina b) => a == b;

        protected override void SetValuesToDefault()
        {
            Current = 0;
            Max = 0;
            Percent = 0;
        }

        protected override void UpdateValues()
        {
            Current = Model.Current;
            Max = Model.Max;
            UpdatePercent();
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.currentChanged += Model_CurrentChanged;
            Model.maxChanged += Model_MaxChanged;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.currentChanged -= Model_CurrentChanged;
            Model.maxChanged -= Model_MaxChanged;
        }

        private void Model_MaxChanged(HeroStamina source, float oldValue, float newValue)
        {
            Max = newValue;
            UpdatePercent();
        }

        private void Model_CurrentChanged(HeroStamina source, float oldValue, float newValue)
        {
            Current = newValue;
            UpdatePercent();
        }

        private void UpdatePercent()
        {
            if (Max == 0)
                Percent = 0;
            else
                Percent = Current / Max;
        }
    }
}
