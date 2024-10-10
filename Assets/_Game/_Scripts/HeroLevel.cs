using SegoLib;

namespace Game
{
    public class HeroLevel : HeroComponent
    {
        private static int GetExperienceNeededToLevel(int level)
        {
            if (level < 0)
                level = 0;

            return (level + 1) * 10;
        }

        public int Level
        {
            get => m_level;
            set => SetLevel(value);
        }
        private int m_level;
        public event ValueChangedWithSourceEvent<HeroLevel, int> levelChanged;

        public int Experience
        {
            get => m_experience;
            set => SetExperience(value);
        }
        private int m_experience;
        public event ValueChangedWithSourceEvent<HeroLevel, int> experienceChanged;

        public int ExperienceNeededToLevel
        {
            get => m_experienceNeededToLevel;
            private set => SetExperienceNeededToLevel(value);
        }
        private int m_experienceNeededToLevel;
        public event ValueChangedWithSourceEvent<HeroLevel, int> experienceNeededToLevelChanged;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized)
                return;

            Initialized = true;
        }

        public void GainExperience(int amount)
        {
            if (amount <= 0) return;

            int newExperience = Experience + amount;

            while(newExperience >= ExperienceNeededToLevel) {
                newExperience -= ExperienceNeededToLevel;
                Level++;
            }

            Experience = newExperience;
        }

        public void SetLevel(int newValue)
        {
            if (newValue < 0)
                newValue = 0;

            if (newValue == m_level) return;

            int oldValue = m_level;
            m_level = newValue;

            ExperienceNeededToLevel = GetExperienceNeededToLevel(m_level);

            levelChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetExperience(int newValue)
        {
            if (newValue < 0)
                newValue = 0;

            if (newValue == m_experience) return;

            int oldValue = m_experience;
            m_experience = newValue;

            experienceChanged?.Invoke(this, oldValue, newValue);
        }

        private void SetExperienceNeededToLevel(int newValue)
        {
            if (newValue < 0)
                newValue = 0;

            if (newValue == m_experienceNeededToLevel) return;

            int oldValue = m_experienceNeededToLevel;
            m_experienceNeededToLevel = newValue;

            experienceNeededToLevelChanged?.Invoke(this, oldValue, newValue);
        }
    } 
}
