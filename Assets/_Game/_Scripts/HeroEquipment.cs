using SegoLib;
using System.Collections.Generic;

namespace Game
{
    public class HeroEquipment : HeroComponent
    {
        public IWeapon MainWeapon
        {
            get => m_mainWeapon;
            private set => SetDominantWeapon(value);
        }
        private IWeapon m_mainWeapon;
        public event ValueChangedWithSourceEvent<HeroEquipment, IWeapon> mainWeaponChanged;

        public IReadOnlyCollection<IWeaponTypeDef> WeaponProficiencies => m_weaponProficiencies;
        private readonly HashSet<IWeaponTypeDef> m_weaponProficiencies = new();

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized)
                return;

            m_weaponProficiencies.UnionWith(Hero.Core.StartingWeaponProficiencies);
            m_weaponProficiencies.Add(Hero.Core.StartingWeapon.WeaponType);

            EquipWeaponAsDominant(Hero.Core.StartingWeapon);

            Initialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_weaponProficiencies.Clear();
            m_mainWeapon = null;
            mainWeaponChanged = null;
        }

        private void SetDominantWeapon(IWeapon newValue)
        {
            if (newValue == m_mainWeapon)
                return;

            IWeapon oldValue = m_mainWeapon;
            m_mainWeapon = newValue;

            mainWeaponChanged?.Invoke(this, oldValue, newValue);
        }

        public bool EquipWeaponAsDominant(IWeapon weapon)
        {
            if (weapon == null || !m_weaponProficiencies.Contains(weapon.WeaponType)) 
                return false;

            SetDominantWeapon(weapon);
            return true;
        }

        public bool IsProficientWithWeaponType(IWeaponTypeDef weaponType) => m_weaponProficiencies.Contains(weaponType);
    } 
}
