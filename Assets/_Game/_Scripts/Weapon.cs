
using SegoLib;
using UnityEngine;

namespace Game
{
    public interface IWeapon : IDisplayName
    {
        public WeaponTypeDef WeaponType { get; }
        public int UpgradeLevel { get; }
        public bool IsUpgradeable { get; }
        public IWeapon GetCopy();
        public void Upgrade();
    }

    [System.Serializable]
    public class Weapon : IWeapon
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public WeaponTypeDef WeaponType { get; private set; }
        [field: SerializeField] public bool IsUpgradeable { get; private set; }
        [field: SerializeField] public int UpgradeLevel { get; private set; }

        public Weapon() { }

        public Weapon(WeaponTypeDef type)
        {
            WeaponType = type;
        }

        public IWeapon GetCopy()
        {
            return new Weapon(WeaponType) { DisplayName = DisplayName, IsUpgradeable = IsUpgradeable, UpgradeLevel = UpgradeLevel };
        }

        public void Upgrade()
        {
            if(IsUpgradeable)
                UpgradeLevel++;
        }
    } 
}
