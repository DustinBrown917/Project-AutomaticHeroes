using SegoLib;
using UnityEngine;

namespace Game
{
    public interface IWeaponTypeDef : IDisplayName, IDisplaySprite
    {
        public HeroSkillDef Skill { get; }
    }

    [CreateAssetMenu(fileName = "New Weapon Type Definition", menuName = "Game/Weapons/WeaponTypeDef")]
    public class WeaponTypeDef : ScriptableObject, IWeaponTypeDef
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Sprite DisplaySprite { get; private set; }
        [field: SerializeField] public HeroSkillDef Skill { get; private set; }
    } 
}
