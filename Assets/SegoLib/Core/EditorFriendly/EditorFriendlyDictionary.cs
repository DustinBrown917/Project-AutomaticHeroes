using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.EditorFriendly
{
    [System.Serializable]
    public class EditorFriendlyDictionary<TKey, TValue> : EditorFriendlyCollection<EditorFriendlyKVP<TKey, TValue>, Dictionary<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public static implicit operator Dictionary<TKey, TValue>(EditorFriendlyDictionary<TKey, TValue> efd) => efd.m_InitializedCollection;

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        protected override Dictionary<TKey, TValue> Initialize(EditorFriendlyKVP<TKey, TValue>[] items)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            for(int i = 0; i < items.Length; i++) {
                dict.Add(items[i].key, items[i].value);
            }

            return dict;
        }

        public bool IsReadOnly => ((IDictionary<TKey, TValue>)m_InitializedCollection).IsReadOnly;

        public ICollection<TKey> Keys => m_InitializedCollection.Keys;
        public ICollection<TValue> Values => m_InitializedCollection.Values;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count => m_InitializedCollection.Count;

        public TValue this[TKey key]
        {
            get => m_InitializedCollection[key];
            set => m_InitializedCollection[key] = value;
        }


        public bool ContainsKey(TKey key) => m_InitializedCollection.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => m_InitializedCollection.TryGetValue(key, out value);
        public void Add(TKey key, TValue value) => m_InitializedCollection.Add(key, value);
        public bool Remove(TKey key) => m_InitializedCollection.Remove(key);
        public void Add(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)m_InitializedCollection).Add(item);
        public void Clear() => m_InitializedCollection.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)m_InitializedCollection).Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)m_InitializedCollection).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)m_InitializedCollection).Remove(item);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_InitializedCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
