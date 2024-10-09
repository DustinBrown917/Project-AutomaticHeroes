using SegoLib.Observables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.EditorFriendly
{
    [System.Serializable]
    public class EditorFriendlyHashSet<T> : EditorFriendlyCollection<T, HashSet<T>>, ICollection<T>, IReadOnlyCollection<T>
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public static implicit operator HashSet<T>(EditorFriendlyHashSet<T> set) => set.m_InitializedCollection;

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        protected override HashSet<T> Initialize(T[] items)
        {
            return new HashSet<T>(m_items);
        }


        public int Count => m_InitializedCollection.Count;
        public bool IsReadOnly => ((ICollection<T>)m_InitializedCollection).IsReadOnly;
        public void Add(T item) => m_InitializedCollection.Add(item);
        public void Clear() => m_InitializedCollection.Clear();
        public bool Contains(T item) => m_InitializedCollection.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => m_InitializedCollection.CopyTo(array, arrayIndex);
        public bool Remove(T item) => m_InitializedCollection.Remove(item);

        public IEnumerator<T> GetEnumerator() => m_InitializedCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    }
}
