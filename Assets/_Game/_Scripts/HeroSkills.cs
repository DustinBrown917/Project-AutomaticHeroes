using System.Collections.Generic;

namespace Game
{
    public class HeroSkills : HeroComponent
    {
        public IReadOnlyList<HeroSkill> Skills => m_skills;
        private HeroSkill[] m_skills;

        public int Count => m_skills.Length;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized)
                return;

            m_skills = new HeroSkill[HeroSkillDef.All.Defs.Count];

            for (int i = 0; i < m_skills.Length; i++) {
                m_skills[i] = new HeroSkill(HeroSkillDef.All.Defs[i]);
            }

            Initialized = true;
        }
    }
}
