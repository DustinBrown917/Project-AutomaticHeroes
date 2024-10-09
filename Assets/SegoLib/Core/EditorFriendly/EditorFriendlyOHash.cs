using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SegoLib.Observables;

namespace SegoLib.EditorFriendly
{
    [System.Serializable]
    public class EditorFriendlyOHash<T> : EditorFriendlyCollection<T, OHash<T>>, IOCollection<T>
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public static implicit operator OHash<T>(EditorFriendlyOHash<T> hash) => hash.m_InitializedCollection;

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        public int Count => m_InitializedCollection.Count;
        public bool IsReadOnly => ((IOCollection<T>)m_InitializedCollection).IsReadOnly;

        public event IOCollection<T>.ItemAddedEvent onItemAdded;
        public event IOCollection<T>.ItemRemovedEvent onItemRemoved;


        protected override OHash<T> Initialize(T[] items)
        {
            OHash<T> collection = new OHash<T>(m_items);

            collection.onItemAdded += onItemAdded;
            collection.onItemRemoved += onItemRemoved;

            return collection;
        }


        public void Add(T item) => base.m_InitializedCollection.Add(item);

        public void Clear() => m_InitializedCollection.Clear();

        public bool Contains(T item) => m_InitializedCollection.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => m_InitializedCollection.CopyTo(array, arrayIndex);

        public bool Remove(T item) => m_InitializedCollection.Remove(item);

        public IEnumerator<T> GetEnumerator() => m_InitializedCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    }
}
