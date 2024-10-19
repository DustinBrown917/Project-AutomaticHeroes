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

        public Sprite Sprite
        {
            get => m_sprite;
            private set => PropertySetter.Set(ref m_sprite, value, m_sprite == value, OnPropertyChanged);
        }
        private Sprite m_sprite;

        protected override bool IsEqual(IHeroBehaviour.IState a, IHeroBehaviour.IState b) => a == b;

        protected override void SetValuesToDefault()
        {
            StateString = string.Empty;
            FlavourString = string.Empty;
            Sprite = null;
        }

        protected override void UpdateValues()
        {
            StateString = Model.StateString;
            FlavourString = Model.FlavourString;
            Sprite = Model.Sprite;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.spriteChanged += Model_SpriteChanged;
            Model.stateStringChanged += Model_StateStringChanged;
            Model.flavourStringChanged += Model_FlavourStringChanged;
            Model.disposed += Model_Disposed;
        }



        protected override void OnModelChangingFromNotNull()
        {
            Model.spriteChanged -= Model_SpriteChanged;
            Model.stateStringChanged -= Model_StateStringChanged;
            Model.flavourStringChanged -= Model_FlavourStringChanged;
            Model.disposed -= Model_Disposed;
        }

        private void Model_FlavourStringChanged(IHeroBehaviour.IState source, string oldValue, string newValue)
        {
            FlavourString = newValue;
        }

        private void Model_SpriteChanged(IHeroBehaviour.IState source, Sprite oldValue, Sprite newValue)
        {
            Sprite = newValue;
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
