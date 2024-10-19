using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Hero Skill Def Library", menuName = "Game/HeroSkillDefLibrary")]
    public class HeroSkillDefLibrary : ScriptableObject
    {
        public IReadOnlyList<HeroSkillDef> Defs => m_defs;
        [SerializeField] private HeroSkillDef[] m_defs;
    }
}
