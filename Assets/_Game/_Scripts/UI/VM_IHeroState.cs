using SegoLib;
using SegoLib.UI.MVVM;
using UnityEngine;

namespace Game.UI
{
    public class VM_IHeroBehaviourState : VM_Base<IHeroBehaviour.IState>
    {
        public string StateString
        {
            get => m_stateString;
            private set => PropertySetter.Set(ref m_stateString, value, m_stateString == value, OnPropertyChanged);
        }
        private string m_stateString;

        public string FlavourString
        {
            get => m_flavourString;
            private set => PropertySetter.Set(ref m_flavourString, value, m_flavourString == value, OnPropertyChanged);
        }
        private string m_flavourString;

        public Sprite ForegroundSprite
        {
            get => m_foregroundSprite;
            private set => PropertySetter.Set(ref m_foregroundSprite, value, m_foregroundSprite == value, OnPropertyChanged);
        }
        private Sprite m_foregroundSprite;

        public Sprite BackgroundSprite
        {
            get => m_backroundSprite;
            private set => PropertySetter.Set(ref m_backroundSprite, value, m_backroundSprite == value, OnPropertyChanged);
        }
        private Sprite m_backroundSprite;

        protected override bool IsEqual(IHeroBehaviour.IState a, IHeroBehaviour.IState b) => a == b;

        protected override void SetValuesToDefault()
        {
            StateString = string.Empty;
            FlavourString = string.Empty;
            ForegroundSprite = null;
            BackgroundSprite = null;
        }

        protected override void UpdateValues()
        {
            StateString = Model.StateString;
            FlavourString = Model.FlavourText;
            ForegroundSprite = Model.ForegroundSprite;
            BackgroundSprite = Model.BackgroundSprite;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.foregroundSpriteChanged += Model_ForegroundSpriteChanged;
            Model.backgroundSpriteChanged += Model_BackgroundSpriteChanged;
            Model.stateStringChanged += Model_StateStringChanged;
            Model.flavourTextChanged += Model_FlavourStringChanged;
            Model.disposed += Model_Disposed;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.foregroundSpriteChanged -= Model_ForegroundSpriteChanged;
            Model.backgroundSpriteChanged -= Model_BackgroundSpriteChanged;
            Model.stateStringChanged -= Model_StateStringChanged;
            Model.flavourTextChanged -= Model_FlavourStringChanged;
            Model.disposed -= Model_Disposed;
        }

        private void Model_FlavourStringChanged(IHeroBehaviour.IState source, string oldValue, string newValue)
        {
            FlavourString = newValue;
        }

        private void Model_ForegroundSpriteChanged(IHeroBehaviour.IState source, Sprite oldValue, Sprite newValue)
        {
            ForegroundSprite = newValue;
        }
        private void Model_BackgroundSpriteChanged(IHeroBehaviour.IState source, Sprite oldValue, Sprite newValue)
        {
            BackgroundSprite = newValue;
        }

        private void Model_StateStringChanged(IHeroBehaviour.IState source, string oldValue, string newValue)
        {
            StateString = newValue;
        }

        private void Model_Disposed(IDisposable obj)
        {
            if (obj == Model)
                SetModel(null);
        }
    }
}
