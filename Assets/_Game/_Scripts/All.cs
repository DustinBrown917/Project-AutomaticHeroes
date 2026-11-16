using SegoLib;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    /// <summary>
    /// Repository for all registered items of type <typeparamref name="T"/>. Should only be used for items that persisty throughout the duration of runtime.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class All<T> where T : IIdentifiable<Identity>
    {
        private static readonly Dictionary<Identity, T> m_allItemsHash = new Dictionary<Identity, T>();
        private static readonly List<T> m_allItems = new List<T>();
        public static int Count => m_allItems.Count;
        public static IReadOnlyList<T> Items => m_allItems;

        public static int NextIndex => Count;

        /// <summary>
        /// Register <paramref name="item"/>. The index of the item in the Items collection is returned in <paramref name="index"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool Register(T item, out int index)
        {
            if (!m_allItemsHash.TryAdd(item.Id, item)) {
                Debug.LogError($"Item with Id {item.Id} already exists.");
                index = -1;
                return false;
            }

            index = Count;
            m_allItems.Add(item);

            return true;
        }

        public static bool TryGetItem(int index, out T item)
        {
            if (index < 0 || index >= m_allItems.Count) {
                item = default;
                return false;
            }

            item = m_allItems[index];
            return true;
        }

        public static bool TryGetItem(Identity identity, out T item)
        {
            return m_allItemsHash.TryGetValue(identity, out item);
        }
    }
}
