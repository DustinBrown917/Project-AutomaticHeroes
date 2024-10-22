using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapons/Weapon")]
    public class ScriptableWeapon : ScriptableObject, IWeapon
    {
        public WeaponTypeDef WeaponType => m_weapon.WeaponType;
        public string DisplayName => m_weapon.DisplayName;
        public int UpgradeLevel => m_weapon.UpgradeLevel;
        public bool IsUpgradeable => m_weapon.IsUpgradeable;

        [SerializeField] private Weapon m_weapon;

        public IWeapon GetCopy() => m_weapon.GetCopy();

        public void Upgrade() => m_weapon.Upgrade();
    } 
}
