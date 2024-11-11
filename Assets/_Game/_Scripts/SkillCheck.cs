using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    [System.Serializable]
    public class SkillCheck
    {
        public enum EType : byte
        {
            Check,
            Contest,
            Absolute
        }

        [field: SerializeField] public HeroSkillDef Skill { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public EType Type { get; private set; }

        public bool HeroPassesCheck(Hero hero, WorldState worldState, out int heroRoll, out int checkRoll)
        {
            switch (Type) {
                case EType.Check: return TestViaCheck(hero, worldState, out heroRoll, out checkRoll);
                case EType.Contest: return TestViaContest(hero, worldState, out heroRoll, out checkRoll);
                case EType.Absolute:
                default: return TestViaAbsolute(hero, worldState, out heroRoll, out checkRoll);
            }
        }

        private bool TestViaCheck(Hero hero, WorldState worldState, out int heroRoll, out int checkRoll)
        {
            heroRoll = hero.Skills[Skill].Check(worldState.Rng);
            checkRoll = Level;

            return heroRoll >= checkRoll;
        }

        private bool TestViaContest(Hero hero, WorldState worldState, out int heroRoll, out int checkRoll)
        {
            heroRoll = hero.Skills[Skill].Contest(worldState.Rng);
            checkRoll = HeroSkill.RollForContest(Level, worldState.Rng);

            return heroRoll >= checkRoll;
        }

        private bool TestViaAbsolute(Hero hero, WorldState worldState, out int heroRoll, out int checkRoll)
        {
            heroRoll = hero.Skills[Skill].Level;
            checkRoll = Level;

            return heroRoll >= checkRoll;
        }
    }
}
