using SegoLib;
using SegoLib.UI.MVVM;

namespace Game.UI
{
    public class VM_IHeroState : VM_Base<IHeroState>
    {
        public float Progress
        {
            get => m_progress;
            private set => PropertySetter.Set(ref m_progress, value, m_progress == value, OnPropertyChanged);
        }
        private float m_progress;

        public string ActionString
        {
            get => m_actionString;
            private set => PropertySetter.Set(ref m_actionString, value, m_actionString == value, OnPropertyChanged);
        }
        private string m_actionString;

        public bool IsComplete
        {
            get => m_isComplete;
            private set => PropertySetter.Set(ref m_isComplete, value, m_isComplete == value, OnPropertyChanged);
        }
        private bool m_isComplete;

        protected override bool IsEqual(IHeroState a, IHeroState b) => a == b;

        protected override void SetValuesToDefault()
        {
            Progress = 0;
            ActionString = string.Empty;
            IsComplete = false;
        }

        protected override void UpdateValues()
        {
            Progress = Model.Progress;
            ActionString = Model.GetActionString();
            IsComplete = Model.StateIsComplete;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.progressChanged += Model_ProgressChanged;
            Model.stateCompleted += Model_StateCompleted;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.progressChanged -= Model_ProgressChanged;
            Model.stateCompleted -= Model_StateCompleted;
        }

        private void Model_StateCompleted(IHeroState state)
        {
            IsComplete = Model.StateIsComplete;
        }

        private void Model_ProgressChanged(IHeroState source, float oldValue, float newValue)
        {
            Progress = newValue;
        }
    }
}
