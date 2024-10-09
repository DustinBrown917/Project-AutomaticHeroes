using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.UI
{
    public class SelectionHandler : MonoBehaviour
    {
        [Serializable]
        public class SelectionChangedEvent : UnityEvent<SelectionHandler> { }

        private HashSet<object> m_multiSelection = new HashSet<object>();
        public IReadOnlyCollection<object> MultiSelection => m_multiSelection;
        public object SingleSelection { get; private set; }

        [SerializeField]
        private bool m_isMultiSelect;
        public bool IsMultiSelect
        {
            get => m_isMultiSelect;
            set => SetIsMultiSelect(value);
        }

        public SelectionChangedEvent onSelectionChanged;

        public void Select(object obj)
        {
            if (!IsMultiSelect) {
                if (SingleSelection != obj) {
                    SingleSelection = obj;
                } else {
                    if (SingleSelection == null) {
                        return;
                    }
                    SingleSelection = null;
                }
            } else {
                if (obj == null) {
                    return;
                }
                if (!m_multiSelection.Remove(obj)) {
                    m_multiSelection.Add(obj);
                }
            }

            onSelectionChanged?.Invoke(this);
        }

        public void SetIsMultiSelect(bool isMultiSelect)
        {
            if (isMultiSelect == IsMultiSelect) {
                return;
            }

            IsMultiSelect = isMultiSelect;

            if (!IsMultiSelect) {
                m_multiSelection.Clear();
            } else {
                SingleSelection = null;
            }
        }

        public bool IsSelected(object obj)
        {
            if(obj == null) return false;

            return m_isMultiSelect ? m_multiSelection.Contains(obj) : SingleSelection == obj;
        }
    }













    public abstract class SelectionHandler<T> : MonoBehaviour
    {
        [Serializable]
        public class SelectionChangedEvent : UnityEvent<SelectionHandler<T>> { }

        [Serializable]
        public class ItemNewlySelectedEvent : UnityEvent<T> { }

        public SelectionChangedEvent onSelectionChanged;
        public ItemNewlySelectedEvent onItemNewlySelected;

        [SerializeField] private bool m_isMultiSelect;
        public bool IsMultiSelect
        {
            get => m_isMultiSelect;
            set => SetIsMultiSelect(value);
        }

        private List<T> m_entireSelection = new List<T>();
        public T LastSelected
        {
            get {
                if(m_entireSelection.Count == 0) {
                    return default;
                } else {
                    return m_entireSelection[^1];
                }
            }
        }

        public IReadOnlyList<T> EntireSelection => m_entireSelection;

        public void Select(T obj)
        {
            if(obj == null) {
                return;
            }

            if(!m_entireSelection.Remove(obj)) {
                if (!IsMultiSelect && m_entireSelection.Count > 0) {
                    m_entireSelection[0] = obj;
                } else {
                    m_entireSelection.Add(obj);
                }
                onItemNewlySelected?.Invoke(obj);
            }

            onSelectionChanged?.Invoke(this);
        }

        public void Deselect(T obj)
        {
            if (m_entireSelection.Remove(obj)) {
                onSelectionChanged?.Invoke(this);
            }
        }


        public bool IsSelected(T obj)
        {
            return m_entireSelection.Contains(obj);
        }

        protected abstract bool ObjectsAreEqual(T obj1, T obj2);

        public void SetIsMultiSelect(bool isMultiSelect)
        {
            if(isMultiSelect == IsMultiSelect) {
                return;
            }

            IsMultiSelect = isMultiSelect;

            if (IsMultiSelect) {
                while(m_entireSelection.Count > 1) {
                    m_entireSelection.RemoveAt(0);
                }
            }
        }
    }


}
