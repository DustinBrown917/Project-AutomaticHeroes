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

        public float Treated
        {
            get => m_treated;
            private set => PropertySetter.Set(ref m_treated, value, m_treated == value, OnPropertyChanged);
        }
        private float m_treated;

        public float TreatedPercent
        {
            get => m_treatedPercent;
            private set => PropertySetter.Set(ref m_treatedPercent, value, m_treatedPercent == value, OnPropertyChanged);
        }
        private float m_treatedPercent;

        protected override bool IsEqual(HeroHealth a, HeroHealth b) => a == b;

        protected override void SetValuesToDefault()
        {
            Current = 0;
            Max = 0;
            Percent = 0;
            Treated = 0;
            TreatedPercent = 0;
        }

        protected override void UpdateValues()
        {
            Current = Model.Current;
            Max = Model.Max;
            Percent = Model.Percent;
            Treated = Model.Treated;
            TreatedPercent = Model.TreatedPercent;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.currentChanged += Model_CurrentChanged;
            Model.maxChanged += Model_MaxChanged;
            Model.treatedChanged += Model_TreatedChanged;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.currentChanged -= Model_CurrentChanged;
            Model.maxChanged -= Model_MaxChanged;
            Model.treatedChanged -= Model_TreatedChanged;
        }

        private void Model_MaxChanged(HeroHealth source, float oldValue, float newValue)
        {
            Max = newValue;
            Percent = Model.Percent;
        }

        private void Model_CurrentChanged(HeroHealth source, float oldValue, float newValue)
        {
            Current = newValue;
            Percent = Model.Percent;
        }

        private void Model_TreatedChanged(HeroHealth source, float oldValue, float newValue)
        {
            Treated = newValue;
            TreatedPercent = (1.0f - Model.Percent) * Model.TreatedPercent;
        }

    } 
}
