using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapons/Weapon")]
    public class ScriptableWeapon : ScriptableObject, IWeapon
    {
        public WeaponTypeDef WeaponType => m_weapon.WeaponType;
        public string DisplayName => m_weapon.DisplayName;
        public int RollBonus => m_weapon.RollBonus;

        [SerializeField] private Weapon m_weapon;
    } 
}
