using SegoLib;
using SegoLib.UI.MVVM;

namespace Game.UI
{
    public class VM_HeroBrain : VM_Base<HeroBrain>
    {
        public IHeroBehaviour.IState CurrentState
        {
            get => m_currentState;
            private set => PropertySetter.Set(ref m_currentState, value, m_currentState == value, OnPropertyChanged);
        }
        private IHeroBehaviour.IState m_currentState;

        protected override bool IsEqual(HeroBrain a, HeroBrain b) => a == b;


        protected override void SetValuesToDefault()
        {
            CurrentState = null;
        }

        protected override void UpdateValues()
        {
            CurrentState = Model.CurrentState;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.stateChanged += Model_StateChanged;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.stateChanged -= Model_StateChanged;
        }

        private void Model_StateChanged(HeroBrain source, IHeroBehaviour.IState oldValue, IHeroBehaviour.IState newValue)
        {
            CurrentState = newValue;
        }
    }
}
