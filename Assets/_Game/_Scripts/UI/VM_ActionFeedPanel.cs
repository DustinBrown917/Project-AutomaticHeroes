using SegoLib.UI.MVVM;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VM_ActionFeedPanel : VM_Base<ActionFeedPanel>
    {
        [SerializeField] private int m_maxLogSize = 5;

        public IEnumerable<ActionFeed.Message> Log
        {
            get => m_log;
            set => SetLog(value);
        }
        private readonly List<ActionFeed.Message> m_log = new();

        protected override bool IsEqual(ActionFeedPanel a, ActionFeedPanel b) => a == b;

        protected override void SetValuesToDefault()
        {
            Log = null;
        }

        protected override void UpdateValues()
        {
            Log = Model.Log;
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.logUpdated += Model_LogUpdated;
        }

        protected override void OnModelChangingFromNotNull()
        {
            Model.logUpdated -= Model_LogUpdated;
        }

        private void Model_LogUpdated()
        {
            Log = Model.Log;
        }

        private void SetLog(IEnumerable<ActionFeed.Message> newValue)
        {
            m_log.Clear();

            if (newValue != null) {
                int count = 0;
                foreach (ActionFeed.Message m in newValue) {
                    m_log.Add(m);

                    count++;
                    if (count == m_maxLogSize)
                        break;
                }
            }

            OnPropertyChanged(nameof(Log));
        }
    } 
}
