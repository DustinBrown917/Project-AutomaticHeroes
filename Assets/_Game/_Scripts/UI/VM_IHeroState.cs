using SegoLib;
using SegoLib.UI.MVVM;
using UnityEngine;

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

        public Sprite Sprite
        {
            get => m_sprite;
            private set => PropertySetter.Set(ref m_sprite, value, m_sprite == value, OnPropertyChanged);
        }
        private Sprite m_sprite;

        protected override bool IsEqual(IHeroState a, IHeroState b) => a == b;

        protected override void SetValuesToDefault()
        {
            Progress = 0;
            ActionString = string.Empty;
            Sprite = null;
        }

        protected override void UpdateValues()
        {
            Progress = Model.Progress;
            ActionString = Model.GetActionString();
            Sprite = Model.StateFactory.Sprite;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.progressChanged += Model_ProgressChanged;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.progressChanged -= Model_ProgressChanged;
        }

        private void Model_ProgressChanged(IHeroState source, float oldValue, float newValue)
        {
            Progress = newValue;
        }
    }
}
