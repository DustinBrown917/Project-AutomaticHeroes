using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SegoLib.Observables
{
    public interface IODict<TKey, TValue> : IOCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> { }

    public class ODict<TKey, TValue> : IODict<TKey, TValue>
    {
        private Dictionary<TKey, TValue> m_dict = new Dictionary<TKey, TValue>();

        public event IOCollection<KeyValuePair<TKey, TValue>>.ItemAddedEvent onItemAdded;
        public event IOCollection<KeyValuePair<TKey, TValue>>.ItemRemovedEvent onItemRemoved;

        public TValue this[TKey key]
        {
            get => m_dict[key];
            set {
                Remove(key);
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => m_dict.Keys;

        public ICollection<TValue> Values => m_dict.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count => m_dict.Count;

        public bool IsReadOnly => ((IDictionary<TKey, TValue>)m_dict).IsReadOnly;

        public virtual void Add(TKey key, TValue value)
        {
            m_dict.Add(key, value);
            onItemAdded?.Invoke(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            m_dict.Add(item.Key, item.Value);
        }

        public virtual void Clear()
        {
            KeyValuePair<TKey, TValue>[] items = m_dict.ToArray();
            foreach (KeyValuePair<TKey, TValue> kvp in items) {
                m_dict.Remove(kvp.Key);
                onItemRemoved?.Invoke(kvp);
            }
            m_dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)m_dict).Contains(item);

        public bool ContainsKey(TKey key) => m_dict.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)m_dict).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_dict.GetEnumerator();

        public virtual bool Remove(TKey key)
        {
            if (m_dict.Remove(key, out TValue value)) {
                onItemRemoved?.Invoke(new KeyValuePair<TKey, TValue>(key, value));
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return m_dict.Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    } 
}
