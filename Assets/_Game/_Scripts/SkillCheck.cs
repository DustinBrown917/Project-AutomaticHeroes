using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    [System.Serializable]
    public class SkillCheck
    {
        [field: SerializeField] public HeroSkillDef Skill { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public bool Absolute { get; private set; }

        public bool HeroPassesCheck(Hero hero, WorldState worldState)
        {
            if (Absolute)
                return hero.Skills[Skill].Level >= Level;
            else
                return hero.Skills.Roll(Skill, worldState) >= Level;
        }
    }
}
