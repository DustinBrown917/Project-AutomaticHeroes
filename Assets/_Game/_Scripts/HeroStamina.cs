using SegoLib;
using UnityEditor.ShaderGraph.Internal;

namespace Game
{
    public class HeroStamina : HeroComponent
    {
        public const float DEFAULT_STAMINA_PER_HOUR = 6.25f;
        public const float DEFAULT_STAMINA_PER_SECOND = DEFAULT_STAMINA_PER_HOUR / 3600f;

        public const float MOVEMENT_STAMINA_PER_HOUR = 8.4f;
        public const float MOVEMENT_STAMINA_PER_SECOND = MOVEMENT_STAMINA_PER_HOUR / 3600f;
        public const float TRAINING_STAMINA_PER_HOUR = 16f;
        public const float TRAINING_STAMINA_PER_SECOND = TRAINING_STAMINA_PER_HOUR / 3600f;

        public const float CULL_STAMINA_PER_HOUR = MOVEMENT_STAMINA_PER_HOUR;
        public const float CULL_STAMINA_PER_SECOND = CULL_STAMINA_PER_HOUR / 3600f;

        public float Current
        {
            get => m_current;
            set => SetCurrent(value);
        }
        private float m_current;
        public event ValueChangedWithSourceEvent<HeroStamina, float> currentChanged;

        public float Max
        {
            get => m_max;
            set => SetMax(value);
        }
        private float m_max;
        public event ValueChangedWithSourceEvent<HeroStamina, float> maxChanged;

        public float Percent
        {
            get => m_percent;
        }
        private float m_percent;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized) return;

            Max = 100;
            Current = Max;

            Initialized = true;
        }

        public void SetCurrent(float newValue)
        {
            if (newValue < 0) newValue = 0;

            if (newValue > Max) newValue = Max;

            if (m_current == newValue)
                return;

            float oldValue = m_current;
            m_current = newValue;

            UpdatePercent();
            currentChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetMax(float newValue)
        {
            if (newValue < 0) newValue = 0;

            if (m_max == newValue)
                return;

            float oldValue = m_max;
            m_max = newValue;

            if (m_max > Current)
                Current = m_max;

            UpdatePercent();
            maxChanged?.Invoke(this, oldValue, newValue);
        }

        public bool TrySpend(float amount)
        {
            if(amount > m_current) return false;
            Current -= amount;

            return true;
        }

        private void UpdatePercent()
        {
            if (Max == 0)
                m_percent = 0;

            m_percent = Current / Max;

        }
    } 
}
