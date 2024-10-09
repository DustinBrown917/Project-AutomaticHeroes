using System;
using System.Collections.Generic;

namespace SegoLib
{
    /// <summary>
    /// Heap, as implemented by good ol' SebLague.
    /// https://github.com/SebLague/Pathfinding/blob/master/Episode%209%20-%20smooth%20path%2002/Assets/Scripts/Heap.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T> where T : IHeapItem<T>
    {

        private readonly List<T> m_items = new List<T>();
        public int Count { get; private set; }

        public Heap() { }

        public void Add(T item)
        {
            item.HeapIndex = Count;
            m_items.Add(item);
            SortUp(item);
            Count++;
        }

        public T RemoveFirst()
        {
            T firstItem = m_items[0];
            Count--;
            m_items[0] = m_items[^1];
            m_items[0].HeapIndex = 0;
            m_items.RemoveAt(m_items.Count - 1);

            if(m_items.Count < 0)
                SortDown(m_items[0]);

            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }


        public bool Contains(T item)
        {
            return Equals(m_items[item.HeapIndex], item);
        }

        void SortDown(T item)
        {
            while (true) {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < Count) {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < Count) {
                        if (m_items[childIndexLeft].CompareTo(m_items[childIndexRight]) < 0) {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(m_items[swapIndex]) < 0) {
                        Swap(item, m_items[swapIndex]);
                    } else {
                        return;
                    }

                } else {
                    return;
                }

            }
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true) {
                T parentItem = m_items[parentIndex];
                if (item.CompareTo(parentItem) > 0) {
                    Swap(item, parentItem);
                } else {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void Swap(T itemA, T itemB)
        {
            m_items[itemA.HeapIndex] = itemB;
            m_items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}
