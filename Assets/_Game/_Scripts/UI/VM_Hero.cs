using SegoLib;
using SegoLib.UI.MVVM;
using System;
using UnityEngine.UI;

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

        public HeroGraphic Graphic
        {
            get => m_graphic;
            private set => PropertySetter.Set(ref m_graphic, value, m_graphic == value, OnPropertyChanged);
        }
        private HeroGraphic m_graphic;

        public HeroHealth Health
        {
            get => m_health;
            private set => PropertySetter.Set(ref m_health, value, m_health == value, OnPropertyChanged);
        }
        private HeroHealth m_health;

        public HeroStamina Stamina
        {
            get => m_stamina;
            private set => PropertySetter.Set(ref m_stamina, value, m_stamina == value, OnPropertyChanged);
        }
        private HeroStamina m_stamina;

        protected override bool IsEqual(Hero a, Hero b) => a == b;

        protected override void SetValuesToDefault()
        {
            Level = null;
            Brain = null;
            Movement = null;
            Color = null;
            Graphic = null;
            Stamina = null;
            Health = null;
        }

        protected override void UpdateValues()
        {
            Level = Model.Level;
            Brain = Model.Brain;
            Movement = Model.Movement;
            Color = Model.Color;
            Graphic = Model.Graphic;
            Stamina = Model.Stamina;
            Health = Model.Health;
        }

        protected override void OnModelChangedToNotNull()
        {

        }

        protected override void OnModelChangingFromNotNull()
        {

        }

    }

}