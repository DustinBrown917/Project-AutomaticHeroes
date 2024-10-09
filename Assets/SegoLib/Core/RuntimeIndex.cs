using System.Collections.Generic;

namespace SegoLib
{
    public interface IRuntimeIndexed
    {
        public RuntimeIndex RuntimeIndex { get; }
    }

    public struct RuntimeIndex
    {
        public static implicit operator int(RuntimeIndex index) => index.Index;

        private static readonly Dictionary<System.Type, int> m_indexedItemCounts = new Dictionary<System.Type, int>();

        public static int Count<T>()
        {
            return Count(typeof(T));
        }

        public static int Count(System.Type type)
        {
            m_indexedItemCounts.TryGetValue(type, out int count);
            return count;
        }

        public static void UpdateIndex<T>(ref RuntimeIndex index)
        {
            UpdateIndex(typeof(T), ref index);
        }
        public static void UpdateIndex(System.Type type, ref RuntimeIndex index)
        {
            if (!m_indexedItemCounts.TryGetValue(type, out int nextIndex)) {
                m_indexedItemCounts[type] = 1;
            } else {
                m_indexedItemCounts[type] = nextIndex + 1;
            }

            index.Index = nextIndex;
        }


        public int Index { get; private set; }
    }
}