using SegoLib;
using UnityEngine;

namespace Game
{
    public class HeroHealth : HeroComponent
    {
        public int Current
        {
            get => m_current;
            set => SetCurrent(value);
        }
        private int m_current;
        public event ValueChangedWithSourceEvent<HeroHealth, int> currentChanged;

        public int Max
        {
            get => m_max;
            set => SetMax(value);
        }
        private int m_max;
        public event ValueChangedWithSourceEvent<HeroHealth, int> maxChanged;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized) return;

            Max = 100;
            Current = Max;

            Initialized = true;
        }

        public void SetCurrent(int newValue)
        {
            if (newValue < 0) newValue = 0;

            if(newValue > Max) newValue = Max;

            if (m_current == newValue)
                return;

            int oldValue = m_current;
            m_current = newValue;

            currentChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetMax(int newValue)
        {
            if (newValue < 0) newValue = 0;

            if (m_max == newValue)
                return;

            int oldValue = m_max;
            m_max = newValue;

            if(Current > Max) 
                Current = Max;

            maxChanged?.Invoke(this, oldValue, newValue);
        }
    } 
}
