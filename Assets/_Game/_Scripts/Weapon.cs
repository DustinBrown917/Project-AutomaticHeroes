
using SegoLib;
using UnityEngine;

namespace Game
{
    public interface IWeapon : IDisplayName
    {
        public WeaponTypeDef WeaponType { get; }
        public int RollBonus { get; }
    }

    [System.Serializable]
    public class Weapon : IWeapon
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public WeaponTypeDef WeaponType { get; private set; }
        [field: SerializeField] public int RollBonus { get; private set; }

        public Weapon() { }

        public Weapon(WeaponTypeDef type)
        {
            WeaponType = type;
        }
    } 
}
