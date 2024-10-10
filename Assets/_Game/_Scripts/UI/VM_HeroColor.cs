using SegoLib;
using SegoLib.UI.MVVM;
using UnityEngine;

namespace Game.UI
{
    public class VM_HeroColor : VM_Base<HeroColor>
    {
        public Color Primary
        {
            get => m_primary;
            set => PropertySetter.Set(ref m_primary, value, m_primary == value, OnPropertyChanged);
        }
        private Color m_primary;

        public Color Secondary
        {
            get => m_secondary;
            set => PropertySetter.Set(ref m_secondary, value, m_secondary == value, OnPropertyChanged);
        }
        private Color m_secondary;

        protected override bool IsEqual(HeroColor a, HeroColor b) => a == b;

        protected override void SetValuesToDefault()
        {
            Primary = Color.black;
            Secondary = Color.white;
        }

        protected override void UpdateValues()
        {
            Primary = Model.Primary;
            Secondary = Model.Secondary;
        }

    }
}
