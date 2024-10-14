using SegoLib;

namespace Game
{
    public class HeroStamina : HeroComponent
    {
        public const float MOVEMENT_STAMINA_PER_SECOND = 8.4f / 3600f;
        public const float TRAINING_STAMINA_PER_SECOND = 16f / 3600f;

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

            maxChanged?.Invoke(this, oldValue, newValue);
        }

        public bool TrySpend(float amount)
        {
            if(amount > m_current) return false;
            Current -= amount;

            return true;
        }
    } 
}
