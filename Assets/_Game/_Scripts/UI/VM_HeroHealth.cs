using SegoLib;
using SegoLib.UI.MVVM;

namespace Game.UI
{
    public class VM_HeroHealth : VM_Base<HeroHealth>
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

        protected override bool IsEqual(HeroHealth a, HeroHealth b) => a == b;

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

        private void Model_MaxChanged(HeroHealth source, int oldValue, int newValue)
        {
            Max = newValue;
            UpdatePercent();
        }

        private void Model_CurrentChanged(HeroHealth source, int oldValue, int newValue)
        {
            Current = newValue;
            UpdatePercent();
        }

        private void UpdatePercent()
        {
            if (Max == 0)
                Percent = 0;
            else
                Percent = (float)Current / Max;
        }
    } 
}
