using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    [System.Serializable]
    public class ProximityList<T>
    {
        [System.Serializable]
        private struct ProximityKey
        {
            public float position;
            public T item;

            public ProximityKey(float position, T item)
            {
                this.position = position;
                this.item = item;
            }
        }

        [SerializeField] private List<ProximityKey> m_items = new List<ProximityKey>();

        public void AddItem(T item, float position)
        {
            m_items.Add(new ProximityKey(position, item));
        }

        public T GetItem(float position)
        {
            T item = default;

            float currentDistance = float.MaxValue;
            float newDistance;

            for (int i = 0; i <= m_items.Count; i++) {
                newDistance = Mathf.Abs(position - m_items[i].position);
                if (newDistance < currentDistance) {
                    currentDistance = newDistance;
                    item = m_items[i].item;
                }
            }

            return item;
        }
    }
}
