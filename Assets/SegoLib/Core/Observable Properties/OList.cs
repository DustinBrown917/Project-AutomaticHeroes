using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Observables
{
    public interface IOList<T> : IList<T>, IReadOnlyList<T>, IOCollection<T> { }

    public class OList<T> : IOList<T>
    {
        private List<T> m_list = new List<T>();

        public event IOCollection<T>.ItemAddedEvent onItemAdded;
        public event IOCollection<T>.ItemRemovedEvent onItemRemoved;

        public virtual T this[int index] { get => m_list[index]; set => Replace(index, value); }

        public virtual int Count => m_list.Count;

        public virtual bool IsReadOnly => false;



        public virtual void Add(T item)
        {
            m_list.Add(item);
            onItemAdded?.Invoke(item);
        }

        public virtual void Clear()
        {
            while (m_list.Count > 0) {
                RemoveAt(0);
            }
            m_list.Clear();
        }

        public virtual bool Contains(T item) => m_list.Contains(item);

        public virtual void CopyTo(T[] array, int arrayIndex) => m_list.CopyTo(array, arrayIndex);

        public virtual IEnumerator<T> GetEnumerator() => m_list.GetEnumerator();

        public virtual int IndexOf(T item) => m_list.IndexOf(item);

        public virtual void Insert(int index, T item)
        {
            m_list.Insert(index, item);
            onItemAdded?.Invoke(item);
        }

        public virtual bool Remove(T item)
        {
            if (m_list.Remove(item)) {
                onItemRemoved?.Invoke(item);
            }

            return false;
        }

        public virtual void RemoveAt(int index)
        {
            T item = m_list[index];
            m_list.RemoveAt(index);
            onItemRemoved?.Invoke(item);
        }

        public virtual void Replace(int index, T item)
        {
            T oldItem = m_list[index];
            m_list[index] = item;

            onItemRemoved?.Invoke(oldItem);
            onItemAdded?.Invoke(item);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    } 
}

