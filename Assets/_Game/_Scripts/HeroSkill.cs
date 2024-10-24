using SegoLib;
using System;

namespace Game
{
    public class HeroSkill : SegoLib.IDisposable
    {
        public static int GetExperienceNeededToLevel(int currentLevel)
        {
            return (currentLevel + 1) * 10;
        }

        public HeroSkillDef Def { get; }

        public int Level
        {
            get => m_level;
            set => SetLevel(value);
        }
        private int m_level = 1;
        public event ValueChangedWithSourceEvent<HeroSkill, int> levelChanged;

        public int ExperienceNeededToLevel
        {
            get => m_experienceNeededToLevel;
            private set => SetExperienceNeededToLevel(value);
        }
        private int m_experienceNeededToLevel = GetExperienceNeededToLevel(1);
        public event ValueChangedWithSourceEvent<HeroSkill, int> experienceNeededToLevelChanged;

        public float Experience
        {
            get => m_experience;
            private set => SetExperience(value);
        }
        private float m_experience;
        public event ValueChangedWithSourceEvent<HeroSkill, float> experienceChanged;

        public float ExperienceGainMultiplier
        {
            get => m_experienceGainMultiplier;
            set => SetExperienceGainMultiplier(value);
        }


        private float m_experienceGainMultiplier = 1.0f;
        public event ValueChangedWithSourceEvent<HeroSkill, float> experienceGainMultiplierChanged;



        public bool IsDisposed { get; private set; }
        public event Action<SegoLib.IPrivatelyDisposable> disposed;

        public HeroSkill(HeroSkillDef def)
        {
            Def = def;
        }

        public void GainExperience(float exp)
        {
            float newExp = Experience + (exp * m_experienceGainMultiplier);

            while (newExp >= ExperienceNeededToLevel) {
                newExp -= ExperienceNeededToLevel;
                Level++;
            }

            Experience = newExp;
        }

        public void SetLevel(int newValue)
        {
            if (newValue < 1)
                newValue = 1;

            if (newValue == m_level)
                return;

            int oldValue = m_level;
            m_level = newValue;

            ExperienceNeededToLevel = GetExperienceNeededToLevel(m_level);

            levelChanged?.Invoke(this, oldValue, m_level);
        }

        private void SetExperienceNeededToLevel(int newValue)
        {
            if (newValue < 1)
                newValue = 1;

            if (newValue == m_experienceNeededToLevel)
                return;

            int oldValue = m_experienceNeededToLevel;
            m_experienceNeededToLevel = newValue;

            Experience = 0;

            experienceNeededToLevelChanged?.Invoke(this, oldValue, newValue);
        }

        private void SetExperience(float newValue)
        {
            if (newValue == m_experience)
                return;

            float oldValue = m_experience;
            m_experience = newValue;

            experienceChanged?.Invoke(this, oldValue, m_experience);
        }

        public void SetExperienceGainMultiplier(float newValue)
        {
            if(newValue < 0)
                newValue = 0;

            if (newValue == m_experienceGainMultiplier)
                return;

            float oldValue = m_experienceGainMultiplier;
            m_experienceGainMultiplier = newValue;

            experienceGainMultiplierChanged?.Invoke(this, oldValue, newValue);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            levelChanged = null;
            experienceChanged = null;
            experienceGainMultiplierChanged = null;
            experienceNeededToLevelChanged = null;

            disposed?.Invoke(this);
            IsDisposed = true;
            disposed = null;
        }
    }
}
