using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// Just an object pool of string builders. Nothing special.
    /// </summary>
    public static class StringBuilderPool 
    {
        private static List<StringBuilder> m_pool = new List<StringBuilder>();
        private static HashSet<StringBuilder> m_poolHash = new HashSet<StringBuilder>();

        /// <summary>
        /// Borrow a string builder from the pool.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder Borrow()
        {
            if(m_pool.Count == 0) {
                m_pool.Add(new StringBuilder());
            }

            StringBuilder builder = m_pool[0];
            m_pool.RemoveAt(0);
            m_poolHash.Remove(builder);
            return builder;
        }

        /// <summary>
        /// Clear and return <paramref name="builder"/> to the pool.
        /// </summary>
        /// <param name="builder"></param>
        public static void Return(StringBuilder builder)
        {
            if (m_poolHash.Contains(builder))
                return;

            builder.Clear();
            m_pool.Add(builder);
            m_poolHash.Add(builder);
        }
    }
}
