using NaughtyAttributes;
using UnityEngine;

namespace Game
{


    public class HeroHeuristics : HeroComponent
    {
        [field: SerializeField] public HighLowHeuristic CurrentHealthHeuristic { get; private set; } = new();
        [field: SerializeField] public HighLowHeuristic ExhaustionHeuristic { get; private set; } = new();

        public bool IsCurrentHealthHigh() => CurrentHealthHeuristic.IsHigh(Hero.Health.Percent);
        public bool IsCurrentHealthLow() => CurrentHealthHeuristic.IsLow(Hero.Health.Percent);

        public bool IsExhaustionHigh() => ExhaustionHeuristic.IsHigh(Hero.Stamina.ExhaustionPercent);
        public bool IsExhaustionLow() => ExhaustionHeuristic.IsLow(Hero.Stamina.ExhaustionPercent);
    }




    [System.Serializable]
    public class HighLowHeuristic
    {
        public float High
        {
            get => m_high;
            set => SetHigh(value);
        }
        [SerializeField, OnValueChanged(nameof(ClampHigh))] private float m_high = 0.7f;


        public float Low
        {
            get => m_low;
            set => SetLow(value);
        }
        [SerializeField, OnValueChanged(nameof(ClampLow))]private float m_low = 0.3f;

        public void SetHigh(float newValue)
        {
            m_high = newValue;
            ClampHigh();
        }

        private void ClampHigh() => m_high = Mathf.Clamp(m_high, m_low, 1.0f);

        public void SetLow(float newValue)
        {
            m_low = newValue;
            ClampLow();
        }

        private void ClampLow() => m_low = Mathf.Clamp(m_low, 0, m_high);

        public bool IsHigh(float value) => value >= m_high;
        public bool IsLow(float value) => value <= m_low;
    }
}
