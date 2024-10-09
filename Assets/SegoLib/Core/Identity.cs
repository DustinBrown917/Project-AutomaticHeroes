using System;
using System.Collections.Generic;

namespace SegoLib
{
    /// <summary>
    /// Used for constant unique identifiers based on strings. Faster than actual strings for equality comparison and hashing, but only uses reference equality.
    /// </summary>
    public abstract class Identity : IEquatable<Identity>, IComparable<Identity>
    {
        public static implicit operator string(Identity identity) => identity.id;

        public static Identity Empty { get; } = Identity<Identity>.Get("");

        public abstract Type Context { get; }
        public readonly string id;
        private int m_hashCode;

        protected Identity(string id)
        {
            this.id = id;
            m_hashCode = id.GetHashCode();
        }

        public override string ToString()
        {
            return id;
        }

        public override bool Equals(object obj)
        {
            if(obj is Identity other) 
                return Equals(other); 
            return false;
        }

        public bool Equals(Identity other)
        {
            if (other == null) return false;
            else return this == other;
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        public int CompareTo(Identity other)
        {
            return id.CompareTo(other.id);
        }
    }

    public class Identity<T> : Identity, IEquatable<Identity<T>>
    {
        public static implicit operator string(Identity<T> identity) => identity.id;
        public static implicit operator Identity<T>(string str) => Get(str);

        private static Dictionary<string, Identity<T>> m_identities = new Dictionary<string, Identity<T>>();


        public static Identity<T> Get(string id)
        {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));

            if (!m_identities.TryGetValue(id, out Identity<T> identity)) {
                identity = new Identity<T>(id);
                m_identities.Add(id, identity);
            }

            return identity;
        }

        /// <summary>
        /// Gets a new id using <paramref name="id"/>. If an id of this type already exists, an ArgumentException is thrown.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Identity<T> GetNew(string id)
        {
            if (id == null)
                throw new System.ArgumentNullException(nameof(id));

            if (m_identities.ContainsKey(id))
                throw new System.ArgumentException($"An Identity of type {typeof(Identity<T>)} with the identifier {id} already exists.", nameof(id));

            Identity<T> identity = new Identity<T>(id);
            m_identities[id] = identity;

            return identity;
        }

        public override Type Context => typeof(T);

        protected Identity(string id) : base(id)
        {
        }

        public override bool Equals(object obj)
        {
            if(obj is Identity<T> other) 
                return Equals(other); 
            return false;
        }

        public bool Equals(Identity<T> other)
        {
            if (other == null) return false;
            else return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
