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
            ActionString = Model.ActionString;
            Sprite = Model.Sprite;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.progressChanged += Model_ProgressChanged;
            Model.spriteChanged += Model_SpriteChanged;
            Model.actionStringChanged += Model_ActionStringChanged;
            Model.disposed += Model_Disposed;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.progressChanged -= Model_ProgressChanged;
            Model.spriteChanged -= Model_SpriteChanged;
            Model.actionStringChanged -= Model_ActionStringChanged;
            Model.disposed -= Model_Disposed;
        }

        private void Model_ProgressChanged(IHeroState source, float oldValue, float newValue)
        {
            Progress = newValue;
        }

        private void Model_SpriteChanged(IHeroState source, Sprite oldValue, Sprite newValue)
        {
            Sprite = newValue;
        }

        private void Model_ActionStringChanged(IHeroState source, string oldValue, string newValue)
        {
            ActionString = newValue;
        }

        private void Model_Disposed(IDisposable obj)
        {
            if (obj == Model)
                SetModel(null);
        }
    }
}
