using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Observables
{
    public interface IOCollection<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        public delegate void ItemAddedEvent(T item);
        public delegate void ItemRemovedEvent(T item);

        public event ItemAddedEvent onItemAdded;
        public event ItemRemovedEvent onItemRemoved;
    } 
}
