using SegoLib;
using SegoLib.Random;
using System;

namespace Game
{
    public class HeroSkill : SegoLib.IDisposable
    {
        /// <summary>
        /// Global tuning value to scale exp gain without changing where/how heroes get exp.
        /// </summary>
        private const float GLOBAL_EXP_GAIN_MULTIPLIER = 10.0f;

        public static int RollForCheck(int skillLevel, IRandomNumberGenerator rng)
        {
            return rng.Range(0, skillLevel * 2 + 1);
        }

        public static int RollForContest(int skillLevel, IRandomNumberGenerator rng)
        {
            return rng.Range(0, skillLevel + 1);
        }

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
        public event Action<SegoLib.IPrivatelyDisposable> Disposed;

        public HeroSkill(HeroSkillDef def)
        {
            Def = def;
        }

        public void GainExperience(float exp)
        {
            float newExp = Experience + (exp * m_experienceGainMultiplier * GLOBAL_EXP_GAIN_MULTIPLIER);

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

        public int Check(IRandomNumberGenerator rng)
        {
            return RollForCheck(Level, rng);
        }

        public int Contest(IRandomNumberGenerator rng)
        {
            return RollForContest(Level, rng);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            levelChanged = null;
            experienceChanged = null;
            experienceGainMultiplierChanged = null;
            experienceNeededToLevelChanged = null;

            Disposed?.Invoke(this);
            IsDisposed = true;
            Disposed = null;
        }
    }
}
