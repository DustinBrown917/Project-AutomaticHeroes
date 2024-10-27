using SegoLib;
using UnityEngine;

namespace Game
{
    public class HeroHealth : HeroComponent
    {
        public const float HEALTH_PERCENT_RECOVERED_PER_HOUR = 0.01f;
        public const float HEALTH_PERCENT_RECOVERED_PER_SECOND = HEALTH_PERCENT_RECOVERED_PER_HOUR / 3600;

        public float Current
        {
            get => m_current;
            set => SetCurrent(value);
        }
        private float m_current = 1;
        public event ValueChangedWithSourceEvent<HeroHealth, float> currentChanged;

        public float Max
        {
            get => m_max;
            set => SetMax(value);
        }
        private float m_max = 1;
        public event ValueChangedWithSourceEvent<HeroHealth, float> maxChanged;

        public float Percent { get; private set; }
        public float MissingHealth => Max - Current;

        public float Treated
        {
            get => m_treated;
            set => SetTreated(value);
        }
        private float m_treated;
        public event ValueChangedWithSourceEvent<HeroHealth, float> treatedChanged;

        public float TreatedPercent { get; private set; }

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized) return;

            Max = 100;
            Current = Max;
            UpdateTreatedPercent();

            Initialized = true;
        }

        public void Damage(float damage)
        {
            if (damage <= 0)
                return;

            Current -= damage;
        }

        public void Recover(float deltaSeconds)
        {
            if (Current >= Max) return;

            float recovery = GetRecoveryPerSecond() * deltaSeconds;
            recovery += Mathf.Min(recovery * 0.75f, Treated);
            Treated -= recovery;
            Current += recovery;
        }

        public void SetCurrent(float newValue)
        {
            if (newValue < 0) newValue = 0;

            if(newValue > Max) newValue = Max;

            if (m_current == newValue)
                return;

            float oldValue = m_current;
            m_current = newValue;

            UpdateTreated();
            UpdatePercent();
            UpdateTreatedPercent();
            currentChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetMax(float newValue)
        {
            if (newValue < 1) newValue = 1;

            if (m_max == newValue)
                return;

            float oldValue = m_max;
            m_max = newValue;

            if(Current > Max) 
                Current = Max;

            UpdateTreated();
            UpdatePercent();
            UpdateTreatedPercent();
            maxChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetTreated(float newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, Max - Current);

            if (newValue == m_treated)
                return;

            float oldValue = m_treated;
            m_treated = newValue;

            UpdateTreatedPercent();
            treatedChanged?.Invoke(this, oldValue, newValue);
        }

        private void UpdateTreated()
        {
            if (Treated > Max - Current)
                Treated = Max - Current;
        }

        private void UpdatePercent()
        {
            if (Max == 0) {
                Percent =  0;
            } else {
                Percent = Current / Max;
            }
        }

        private void UpdateTreatedPercent()
        {
            float maxTreated = Max - Current;
            if (maxTreated == 0)
                TreatedPercent = 0;
            else
                TreatedPercent = Treated / maxTreated;
        }

        public bool NeedsTreatment()
        {
            float missingHealth = Max - Current;

            return Treated + float.Epsilon < missingHealth;
        }

        private float GetRecoveryPerSecond()
        {
            return HEALTH_PERCENT_RECOVERED_PER_SECOND * Max;
        }
    } 
}
