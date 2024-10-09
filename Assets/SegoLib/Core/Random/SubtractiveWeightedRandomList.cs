using System.Collections.Generic;

namespace SegoLib.Random
{
    public class SubtractiveWeightedRandomList<T>
    {
        private readonly Dictionary<T, float> m_items = new Dictionary<T, float>();
        private float m_sum = 0;
        private bool m_sumNeedsRefresh;

        public void SetItemWeight(T item, float weight)
        {
            if (weight <= 0) {
                RemoveItem(item);
                return;
            }

            m_items[item] = weight;
            m_sumNeedsRefresh = true;
        }

        public void ModifyItemWeight(T item, float weight)
        {
            m_items.TryGetValue(item, out float existingWeight);

            SetItemWeight(item, existingWeight + weight);
        }

        public float GetItemWeight(T item)
        {
            m_items.TryGetValue(item, out float weight);

            return weight;
        }

        public bool RemoveItem(T item)
        {
            if (m_items.Remove(item, out float weight)) {
                m_sum -= weight;
                return true;
            }

            return false;
        }

        private void RefreshSum()
        {
            m_sum = 0;
            foreach (KeyValuePair<T, float> pair in m_items) {
                m_sum += pair.Value;
            }
        }


        /// <summary>
        /// Get a value randomly from this list.
        /// </summary>
        /// <param name="randomValue">Random value between 0 and 1 (inclusive)</param>
        /// <returns>False if the list is empty and an item could not be retrieved. Otherwise, true.</returns>
        public bool GetItem(float randomValueBetween0And1, out T randomItem)
        {
            if (m_sumNeedsRefresh)
                RefreshSum();

            float val = randomValueBetween0And1 * m_sum;

            randomItem = default(T);
            if (m_items.Count == 0)
                return false;

            foreach (KeyValuePair<T, float> kvp in m_items) {
                randomItem = kvp.Key;
                val -= kvp.Value;
                if (val <= 0)
                    break;
            }

            return true;
        }
    }
}
