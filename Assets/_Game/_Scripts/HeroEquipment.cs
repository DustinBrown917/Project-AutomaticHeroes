using SegoLib;

namespace Game
{
    public class HeroEquipment : HeroComponent
    {
        public IWeapon MainWeapon
        {
            get => m_mainWeapon;
            set => SetDominantWeapon(value);
        }
        private IWeapon m_mainWeapon;
        public event ValueChangedWithSourceEvent<HeroEquipment, IWeapon> mainWeaponChanged;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized)
                return;

            SetDominantWeapon(Hero.Core.UnnarmedWeapon);

            Initialized = true;
        }

        public void SetDominantWeapon(IWeapon newValue)
        {
            if(newValue == null)
                newValue = Hero.Core.UnnarmedWeapon;

            if (newValue == m_mainWeapon)
                return;

            IWeapon oldValue = m_mainWeapon;
            m_mainWeapon = newValue;

            mainWeaponChanged?.Invoke(this, oldValue, newValue);
        }

    } 
}
