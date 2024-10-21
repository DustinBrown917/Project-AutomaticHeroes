using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ScriptableLibrary<T> : ScriptableObject, IEnumerable<T>
    {
        public T this[int index] => m_items[index];

        public IReadOnlyList<T> Items => m_items;
        [SerializeField] private T[] m_items;

        public int Count => m_items.Length;

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)m_items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
