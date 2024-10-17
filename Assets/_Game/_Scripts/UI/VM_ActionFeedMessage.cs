using SegoLib;
using SegoLib.UI.MVVM;

namespace Game.UI
{
    public class VM_ActionFeedMessage : VM_Base<ActionFeed.Message>
    {
        public string Message
        {
            get => m_message;
            set => PropertySetter.Set(ref m_message, value, m_message == value, OnPropertyChanged);
        }
        private string m_message = string.Empty;

        protected override bool IsEqual(ActionFeed.Message a, ActionFeed.Message b) => a == b;

        protected override void SetValuesToDefault()
        {
            Message = string.Empty;
        }

        protected override void UpdateValues()
        {
            Message = Model.message;
        }
    }
}
