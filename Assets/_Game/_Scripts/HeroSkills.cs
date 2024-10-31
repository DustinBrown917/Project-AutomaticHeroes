using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public class HeroSkills : HeroComponent, IEnumerable<HeroSkill>
    {
        public HeroSkill this[HeroSkillDef def] => this[def.RuntimeIndex];
        public HeroSkill this[int index] => m_skills[index];

        public IReadOnlyList<HeroSkill> SkillList => m_skills;
        private HeroSkill[] m_skills = System.Array.Empty<HeroSkill>();
        public int Count => m_skills.Length;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized)
                return;

            m_skills = new HeroSkill[HeroSkillDef.All.Count];

            for (int i = 0; i < m_skills.Length; i++) {
                m_skills[i] = new HeroSkill(HeroSkillDef.All[i]);
                m_skills[i].levelChanged += OnSkillLevelChanged;
            }

            Initialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach(HeroSkill skill in m_skills) {
                skill.Dispose();
            }

            m_skills = System.Array.Empty<HeroSkill>();
        }

        private void OnSkillLevelChanged(HeroSkill source, int oldValue, int newValue)
        {
            ActionFeed.Post($"{Hero.Core.ColoredName} increased their {source.Def.DisplayName} skill to {newValue}!");
            Hero.Level.GainExperience(1.0f);
        }

        public int Roll(HeroSkillDef skill, WorldState worldState)
        {
            return worldState.Rng.Range(0, this[skill].Level * 2 + 1);
        }

        public IEnumerator<HeroSkill> GetEnumerator() => ((IEnumerable<HeroSkill>)m_skills).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
