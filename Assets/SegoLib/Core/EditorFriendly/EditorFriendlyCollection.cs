using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.EditorFriendly
{
    public abstract class EditorFriendlyCollection<TItem, TCollection>
    {
        [Tooltip("If true, the item collection will be purged when the wrapped collection is initialized.")]
        [SerializeField] private bool m_purgeItemsOnInitialize;
        [SerializeField] protected TItem[] m_items;
        [System.NonSerialized] private bool m_initialized = false;
        private TCollection m_baseCollection;
        protected TCollection m_InitializedCollection
        {
            get {
                if(!m_initialized) {
                    m_baseCollection = Initialize(m_items);

                    if (m_purgeItemsOnInitialize)
                        m_items = null;

                    m_initialized = true;
                }
                    

                return m_baseCollection;
            }
        }

        protected abstract TCollection Initialize(TItem[] items);
    } 
}
