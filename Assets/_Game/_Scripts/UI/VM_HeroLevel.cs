using SegoLib;
using SegoLib.UI.MVVM;

namespace Game.UI
{
    public class VM_HeroLevel : VM_Base<HeroLevel>
    {
        public int Level
        {
            get => m_level;
            private set => PropertySetter.Set(ref m_level, value, m_level == value, OnPropertyChanged);
        }
        private int m_level;

        public int Experience
        {
            get => m_experience;
            private set => PropertySetter.Set(ref m_experience, value, m_experience == value, OnPropertyChanged);
        }
        private int m_experience;

        public int ExperienceNeededToLevel
        {
            get => m_experienceNeededToLevel;
            private set => PropertySetter.Set(ref m_experienceNeededToLevel, value, m_experienceNeededToLevel == value, OnPropertyChanged);
        }
        private int m_experienceNeededToLevel;

        public float Progress
        {
            get => m_progress; 
            private set => PropertySetter.Set(ref m_progress, value, m_progress == value, OnPropertyChanged);
        }
        private float m_progress;

        protected override bool IsEqual(HeroLevel a, HeroLevel b) => a == b;

        protected override void SetValuesToDefault()
        {
            Level = 0;
            Experience = 0;
            ExperienceNeededToLevel = 0;
            Progress = 0;
        }

        protected override void UpdateValues()
        {
            Level = Model.Level;
            Experience = Model.Experience;
            ExperienceNeededToLevel = Model.ExperienceNeededToLevel;
            Progress = GetProgress();
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.levelChanged += Model_LevelChanged;
            Model.experienceChanged += Model_ExperienceChanged;
            Model.experienceNeededToLevelChanged += Model_ExperienceNeededToLevelChanged;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.levelChanged -= Model_LevelChanged;
            Model.experienceChanged -= Model_ExperienceChanged;
            Model.experienceNeededToLevelChanged -= Model_ExperienceNeededToLevelChanged;
        }

        private void Model_ExperienceNeededToLevelChanged(HeroLevel source, int oldValue, int newValue)
        {
            ExperienceNeededToLevel = newValue;
            Progress = GetProgress();
        }

        private void Model_ExperienceChanged(HeroLevel source, int oldValue, int newValue)
        {
            Experience = newValue;
            Progress = GetProgress();
        }

        private void Model_LevelChanged(HeroLevel source, int oldValue, int newValue)
        {
            Level = newValue;
        }

        private float GetProgress() => (float)Experience / ExperienceNeededToLevel;
    }

}