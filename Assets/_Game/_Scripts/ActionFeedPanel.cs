using Game.UI;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game
{
    public class ActionFeedPanel : MonoBehaviour
    {
        [SerializeField] private int m_maxLogSize = 100;
        [SerializeField] private VM_ActionFeedPanel m_targetVm;

        public IReadOnlyList<ActionFeed.Message> Log => m_log;
        private readonly List<ActionFeed.Message> m_log = new();
        public event Action logUpdated;

        private void Start()
        {
            if(m_targetVm != null)
                m_targetVm.SetModel(this);

            ActionFeed.messagePosted += ActionFeed_MessagePosted;
        }

        private void OnDestroy()
        {
            if(m_targetVm != null) {
                m_targetVm.SetModel(null);
                m_targetVm = null;
            }

            ActionFeed.messagePosted -= ActionFeed_MessagePosted;
            logUpdated = null;
            m_log.Clear();
        }

        private void ActionFeed_MessagePosted(ActionFeed.Message obj)
        {
            m_log.Insert(0, obj);

            if(m_log.Count > m_maxLogSize) {
                m_log.RemoveAt(m_log.Count - 1);
            }

            logUpdated?.Invoke();
        }
    } 
}
