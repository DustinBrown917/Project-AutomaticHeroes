using System.Collections.Generic;

namespace SegoLib
{
    /// <summary>
    /// A string wrapper used for fast hashing and reference comparison.
    /// </summary>
    public class FastHashString
    {
        public static FastHashString Empty { get; }
        private static Dictionary<string, FastHashString> m_fastStrings = new Dictionary<string, FastHashString>();

        static FastHashString()
        {
            Empty = Get("");
        }

        public static FastHashString Get(string s)
        {
            if (m_fastStrings.TryGetValue(s, out FastHashString fastHashString)) {
                return fastHashString;
            }

            fastHashString = new FastHashString(s);
            m_fastStrings.Add(s, fastHashString);
            return fastHashString;
        }

        public static bool IsNullOrEmpty(FastHashString fs)
        {
            if (fs == null)
                return true;

            return string.IsNullOrEmpty(fs.value);
        }

        public static implicit operator string(FastHashString s) => s.value;
        public static implicit operator FastHashString(string s) => Get(s);

        private int m_hashCode;
        public readonly string value;

        private FastHashString(string value)
        {
            this.value = value;
            m_hashCode = value.GetHashCode();
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
