using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Observables
{

    public class OHash<T> : IOCollection<T>
    {
        public static implicit operator HashSet<T>(OHash<T> set) => set.m_hash;

        private HashSet<T> m_hash = new HashSet<T>();

        public int Count => m_hash.Count;

        public bool IsReadOnly => ((ICollection<T>)m_hash).IsReadOnly;

        public event IOCollection<T>.ItemAddedEvent onItemAdded;
        public event IOCollection<T>.ItemRemovedEvent onItemRemoved;

        void ICollection<T>.Add(T item) => Add(item);

        public OHash()
        {
            m_hash = new HashSet<T>();
        }

        public OHash(IEnumerable<T> items)
        {
            m_hash = new HashSet<T>(items);
        }

        public bool Add(T item)
        {
            if (m_hash.Add(item)) {
                onItemAdded?.Invoke(item);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            T[] items = new T[m_hash.Count];
            CopyTo(items, 0);
            for(int i = 0; i < items.Length; i++) {
                Remove(items[i]);
            }

            m_hash.Clear();
        }

        public bool Contains(T item) => m_hash.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => m_hash.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            if (m_hash.Remove(item)) {
                onItemRemoved?.Invoke(item);
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator() => m_hash.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    } 
}
