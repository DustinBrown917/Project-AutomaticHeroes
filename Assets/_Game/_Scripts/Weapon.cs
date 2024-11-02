
using SegoLib;
using UnityEngine;

namespace Game
{
    public interface IWeapon : IDisplayName
    {
        public WeaponTypeDef WeaponType { get; }
        public int UpgradeLevel { get; }
        public bool IsUpgradeable { get; }
        public float DamagePerAttack { get; }
        public float AttackCooldown { get; }
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
        [field: SerializeField] public float DamagePerMinute { get; private set; }
        public float DamagePerAttack { get; private set; }
        [field:SerializeField] public float AttackCooldown { get; private set; }

        public Weapon(WeaponTypeDef type, string displayName, bool isUpgradeable, float damagePerMinute, float attackCooldown)
        {
            WeaponType = type;
            DisplayName = displayName;
            IsUpgradeable = isUpgradeable;
            DamagePerMinute = damagePerMinute;
            AttackCooldown = attackCooldown;

            UpdateDamagePerAttack();
        }

        public IWeapon GetCopy()
        {
            return new Weapon(WeaponType, DisplayName, IsUpgradeable, DamagePerMinute, AttackCooldown);
        }

        public void Upgrade()
        {
            if (!IsUpgradeable)
                return;

            UpgradeLevel++;
            DamagePerMinute += 1.0f;
            UpdateDamagePerAttack();
        }

        private void UpdateDamagePerAttack()
        {
            float attacksPerMinute = 60f / AttackCooldown;
            DamagePerAttack = DamagePerMinute / attacksPerMinute;
        }
    } 
}
