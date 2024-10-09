using SegoLib.Observables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.EditorFriendly
{
    [System.Serializable]
    public class EditorFriendlyODict<TKey, TValue> : EditorFriendlyCollection<EditorFriendlyKVP<TKey, TValue>, ODict<TKey, TValue>>, IODict<TKey, TValue>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public static implicit operator ODict<TKey, TValue>(EditorFriendlyODict<TKey, TValue> efOdict) => efOdict.m_InitializedCollection;

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        public bool IsReadOnly => ((IDictionary<TKey, TValue>)m_InitializedCollection).IsReadOnly;

        public ICollection<TKey> Keys => m_InitializedCollection.Keys;
        public ICollection<TValue> Values => m_InitializedCollection.Values;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count => m_InitializedCollection.Count;

        public TValue this[TKey key] {
            get => m_InitializedCollection[key];
            set => m_InitializedCollection[key] = value;
        }

        public event IOCollection<KeyValuePair<TKey, TValue>>.ItemAddedEvent onItemAdded;
        public event IOCollection<KeyValuePair<TKey, TValue>>.ItemRemovedEvent onItemRemoved;

        protected override ODict<TKey, TValue> Initialize(EditorFriendlyKVP<TKey, TValue>[] items)
        {
            ODict<TKey, TValue> collection = new ODict<TKey, TValue>();

            foreach (var kvp in items) {
                collection.Add(kvp.key, kvp.value);
            }

            collection.onItemAdded += onItemAdded;
            collection.onItemRemoved += onItemRemoved;

            return collection;
        }

        public virtual void Add(TKey key, TValue value) => m_InitializedCollection.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> item) => m_InitializedCollection.Add(item);

        public virtual void Clear() => m_InitializedCollection.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)m_InitializedCollection).Contains(item);

        public bool ContainsKey(TKey key) => m_InitializedCollection.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)m_InitializedCollection).CopyTo(array, arrayIndex);

        public virtual bool Remove(TKey key) => m_InitializedCollection.Remove(key);

        public bool Remove(KeyValuePair<TKey, TValue> item) => m_InitializedCollection.Remove(item.Key);

        public bool TryGetValue(TKey key, out TValue value) => m_InitializedCollection.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_InitializedCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    }
}
