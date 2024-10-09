using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.UI
{
    public class TooltipDataProviderComponent : MonoBehaviour, ITooltipDataProvider
    {
        [field: SerializeField] public string TooltipContentType { get; set; }

        private Dictionary<string, object> m_data = new Dictionary<string, object>();

        public event Action<TooltipDataProviderComponent> onDataRequested;

        public void AddData(string field, object data)
        {
            m_data[field] = data;
        }

        public void RemoveData(string field)
        {
            m_data.Remove(field);
        }

        public void ClearData()
        {
            m_data.Clear();
        }

        public Dictionary<string, object> GetTooltipData()
        {
            if (!enabled) {
                return null;
            }

            onDataRequested?.Invoke(this);

            if(m_data.Count == 0) {
                return null;
            }

            return m_data;
        }
    }
}
