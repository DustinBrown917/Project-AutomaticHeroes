using SegoLib;
using SegoLib.UI.MVVM;
using UnityEngine;

namespace Game.UI
{
    public class VM_HeroColor : VM_Base<HeroCore>
    {
        public string Name
        {
            get => m_name;
            set => PropertySetter.Set(ref m_name, value, m_name == value, OnPropertyChanged);
        }
        private string m_name;

        public Color Base
        {
            get => m_base;
            set => PropertySetter.Set(ref m_base, value, m_base == value, OnPropertyChanged);
        }
        private Color m_base;

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

        public Sprite CharacterSprite
        {
            get => m_characterSprite;
            private set => PropertySetter.Set(ref m_characterSprite, value, m_characterSprite == value, OnPropertyChanged);
        }
        private Sprite m_characterSprite;

        public Sprite BannerSprite
        {
            get => m_bannerSprite;
            private set => PropertySetter.Set(ref m_bannerSprite, value, m_bannerSprite == value, OnPropertyChanged);
        }
        private Sprite m_bannerSprite;

        protected override bool IsEqual(HeroCore a, HeroCore b) => a == b;

        protected override void SetValuesToDefault()
        {
            Name = string.Empty;
            Base = Color.gray;
            Primary = Color.black;
            Secondary = Color.white;
            CharacterSprite = null;
            BannerSprite = null;
        }

        protected override void UpdateValues()
        {
            Name = Model.Name;
            Base = Model.Base;
            Primary = Model.Primary;
            Secondary = Model.Secondary;
            CharacterSprite = Model.CharacterSprite;
            BannerSprite = Model.BannerSprite;
        }
    }
}
