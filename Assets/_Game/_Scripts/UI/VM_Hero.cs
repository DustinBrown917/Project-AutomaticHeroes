using SegoLib;
using SegoLib.UI.MVVM;
using System;

namespace Game.UI
{
    public class VM_Hero : VM_Base<Hero>
    {
        public HeroColor Color
        {
            get => m_color;
            set => PropertySetter.Set(ref m_color, value, m_color == value, OnPropertyChanged);
        }
        private HeroColor m_color;

        public HeroLevel Level
        {
            get => m_level;
            private set => PropertySetter.Set(ref m_level, value, m_level == value, OnPropertyChanged);
        }
        private HeroLevel m_level;

        public HeroBrain Brain
        {
            get => m_brain;
            private set => PropertySetter.Set(ref m_brain, value, m_brain == value, OnPropertyChanged);
        }
        private HeroBrain m_brain;

        public HeroMovement Movement
        {
            get => m_movement;
            private set => PropertySetter.Set(ref m_movement, value, m_movement == value, OnPropertyChanged);
        }
        private HeroMovement m_movement;

        protected override bool IsEqual(Hero a, Hero b) => a == b;

        protected override void SetValuesToDefault()
        {
            Level = null;
            Brain = null;
            Movement = null;
            Color = null;
        }

        protected override void UpdateValues()
        {
            Level = Model.Level;
            Brain = Model.Brain;
            Movement = Model.Movement;
            Color = Model.Color;
        }

        protected override void OnModelChangedToNotNull()
        {

        }

        protected override void OnModelChangingFromNotNull()
        {

        }

    }

}