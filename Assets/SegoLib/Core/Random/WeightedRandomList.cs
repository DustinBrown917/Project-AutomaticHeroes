using System.Collections;
using System.Collections.Generic;
using System;

namespace SegoLib.Random
{
    public interface IWeightedRandomList
    {
        public enum EWeightSortMode : byte
        {
            None,
            Ascending,
            Descending
        }
    }

    public interface IWeightedRandomList<T> : IEnumerable<(T item, int weight)> 
    {
        public class WeightComparer : IComparer<(T, int)>
        {

            private Func<(T, int), (T, int), int> m_comparison;

            public WeightComparer()
            {
                MakeAscending();
            }

            public int Compare((T, int) x, (T, int) y)
            {
                return m_comparison(x, y);
            }

            private int CompareAscending((T, int weight) x, (T, int weight) y)
            {
                return x.weight.CompareTo(y.weight);
            }

            private int CompareDescending((T, int weight) x, (T, int weight) y)
            {
                return y.weight.CompareTo(x.weight);
            }

            public void MakeAscending()
            {
                m_comparison = CompareAscending;
            }

            public void MakeDescending()
            {
                m_comparison = CompareDescending;
            }
        }
    }

    public class WeightedRandomList<T> : IWeightedRandomList<T>
    {
        private IWeightedRandomList<T>.WeightComparer m_comparer = new IWeightedRandomList<T>.WeightComparer();

        private readonly Dictionary<T, int> m_indexHash = new Dictionary<T, int>();
        private readonly List<(T item, int weight)> m_itemWeightList = new List<(T item, int weight)>();
        private int m_sumOfItemWeights = 0;

        private int[] m_cumulativeWeights = Array.Empty<int>();

        public IWeightedRandomList.EWeightSortMode WeightSortMode { get; private set; } = IWeightedRandomList.EWeightSortMode.None;

        /// <summary>
        /// Set the weight of a given item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="weight"></param>
        public void SetItemWeight(T item, int weight)
        {
            if (item == null)
                return;

            if (weight <= 0) {
                RemoveItem(item);
                return;
            }

            if (WeightSortMode == IWeightedRandomList.EWeightSortMode.None)
                SetItemWeightUnsorted(item, weight);
            else
                SetItemWeightSorted(item, weight);
        }

        private void SetItemWeightUnsorted(T item, int weight)
        {
            // If the item already exists, set its weight and rebuild cumulative weights from that index forward.
            if (m_indexHash.TryGetValue(item, out int index)) {
                m_itemWeightList[index] = (item, weight);
                RebuildCumulativeWeights(index);
            } else { // If the item is new, add it to the end and rebuild all cumulative weights (because the list size has changed).
                m_indexHash[item] = m_itemWeightList.Count;
                m_itemWeightList.Add((item, weight));
                RebuildCumulativeWeights();
            }
        }

        private void SetItemWeightSorted(T item, int weight)
        {
            (T item, int weight) newPair = (item, weight);

            // If the item already exists here, remove it and track it's original index.
            if (m_indexHash.TryGetValue(item, out int originalIndex)) {
                m_itemWeightList.RemoveAt(originalIndex);
            } else {
                originalIndex = int.MaxValue;
            }

            // Insert the item into the weight list in the specified sorting manner
            int newIndex = m_itemWeightList.OrderedInsert(newPair, m_comparer);

            // The working index will be the lowest between new index and original index.
            int workingIndex = originalIndex < newIndex ? originalIndex : newIndex;

            // If the item is new or changed its location in the list, rebuild the index hash
            if(originalIndex != newIndex) {
                RebuildIndexHash(workingIndex);
            }

            // Rebuild the cumulative weights.
            RebuildCumulativeWeights(workingIndex);
        }

        /// <summary>
        /// Remove an item from this list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(T item)
        {
            if (m_indexHash.Remove(item, out int index)) {
                RemoveFromWeightListAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove the item and weight at <paramref name="index"/>, and perform necessary upkeep operations to maintain the integrity of this list.
        /// </summary>
        /// <param name="index"></param>
        private void RemoveFromWeightListAt(int index)
        {
            m_itemWeightList.RemoveAt(index);
            RebuildIndexHash(index);
            RebuildCumulativeWeights();
        }

        /// <summary>
        /// Rebuilg the cumulative weights for this list.
        /// </summary>
        private void RebuildCumulativeWeights(int startingFromIndex = 0)
        {
            if (m_cumulativeWeights.Length != m_itemWeightList.Count) {
                m_cumulativeWeights = new int[m_itemWeightList.Count];
                startingFromIndex = 0;
            }

            int accumulatedWeight = 0;
            if(startingFromIndex - 1 > 0)
                accumulatedWeight = m_cumulativeWeights[startingFromIndex - 1];

            for (int i = startingFromIndex; i < m_cumulativeWeights.Length; i++) {
                accumulatedWeight += m_itemWeightList[i].weight;
                m_cumulativeWeights[i] = accumulatedWeight;
            }

            m_sumOfItemWeights = accumulatedWeight;
        }

        /// <summary>
        /// Get a random item from this list.
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public T GetRandom(IRandomNumberGenerator rng)
        {
            if (m_itemWeightList.Count == 0)
                return default(T);

            int num = rng.Range(0, m_sumOfItemWeights + 1);

            int index = Array.BinarySearch(m_cumulativeWeights, num);

            if (index < 0)
                index = ~index;

            return m_itemWeightList[index].item;
        }

        /// <summary>
        /// Gets a random item from this list.
        /// </summary>
        /// <param name="between0And1">Float value between 0 and 1 inclusive.</param>
        /// <param name="item">The item retrieved.</param>
        /// <returns>False if the list is empty. Otherwise, true.</returns>
        public bool GetRandom(float between0And1, out T item)
        {
            item = default;
            if(m_itemWeightList.Count == 0) {
                return false;
            }

            int num = (int)(m_sumOfItemWeights * between0And1) + 1;

            int index = Array.BinarySearch(m_cumulativeWeights, num);


            if (index < 0)
                index = ~index;

            item = m_itemWeightList[index].item;
            return true;
        }

        /// <summary>
        /// Get the percent chance of an item with <paramref name="weight"/> being drawn from this list.
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public float GetChanceOfWeight(int weight)
        {
            if (weight <= 0)
                return 0f;

            return (float)weight / m_sumOfItemWeights;
        }

        /// <summary>
        /// Get the weight of <paramref name="item"/> within this list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemWeight(T item)
        {
            if (m_indexHash.TryGetValue(item, out int index)) {
                return m_itemWeightList[index].weight;
            }

            return 0;
        }

        /// <summary>
        /// Get whether or not this list contains <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return m_indexHash.ContainsKey(item);
        }

        /// <summary>
        /// Rebuild the index hash.
        /// </summary>
        /// <param name="startingAtIndex"></param>
        private void RebuildIndexHash(int startingAtIndex = 0)
        {
            for(int i = startingAtIndex; i < m_itemWeightList.Count; i++) {
                m_indexHash[m_itemWeightList[i].item] = i;
            }
        }

        /// <summary>
        /// Sort this list in ascending order by weight.
        /// </summary>
        public void SetWeightSortMode(IWeightedRandomList.EWeightSortMode sortMode)
        {
            if (sortMode == WeightSortMode)
                return;

            WeightSortMode = WeightSortMode;

            switch (sortMode) {
                case IWeightedRandomList.EWeightSortMode.None:
                    return;
                case IWeightedRandomList.EWeightSortMode.Ascending:
                    m_comparer.MakeAscending();
                    m_itemWeightList.Sort(m_comparer);
                    break;
                case IWeightedRandomList.EWeightSortMode.Descending:
                    m_comparer.MakeDescending();
                    m_itemWeightList.Sort(m_comparer);
                    break;
            }

            RebuildCumulativeWeights();
            RebuildIndexHash();
        }

        /// <summary>
        /// Clear this list.
        /// </summary>
        public void Clear()
        {
            m_indexHash.Clear();
            m_itemWeightList.Clear();
            m_sumOfItemWeights = 0;
            m_cumulativeWeights = Array.Empty<int>();
        }

        #region IEnumerator Implementation
        public IEnumerator<(T item, int weight)> GetEnumerator()
        {
            return m_itemWeightList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion /IEnumerator Implementation
    }
}