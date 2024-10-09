
namespace SegoLib
{
    /// <summary>
    /// A storage object to manage objects that should be uniquely identifiable.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public abstract class ObjectLibrary_uint<TObject> : ObjectLibrary<uint, TObject> where TObject : class, IIdentifiable<uint>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObjectLibrary_uint() : base() 
        {
            m_nextId = 1;
        }

        /// <inheritdoc/>
        protected override void UpdateNextId()
        {
            while (m_library.ContainsKey(m_nextId)) m_nextId++;
        }

        /// <inheritdoc/>
        public override bool IdIsEmpty(uint id) => id == 0;
    }

    /// <summary>
    /// A storage object to manage objects that should be uniquely identifiable.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public abstract class ObjectLibrary_int<TObject> : ObjectLibrary<int, TObject> where TObject : class, IIdentifiable<int>
    {
        private int m_emptyIdValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObjectLibrary_int(int emptyIdValue, int startingIdValue) : base()
        {
            m_nextId = startingIdValue;
            m_emptyIdValue = emptyIdValue;
        }

        /// <inheritdoc/>
        protected override void UpdateNextId()
        {
            while (m_library.ContainsKey(m_nextId)) m_nextId++;
        }

        /// <inheritdoc/>
        public override bool IdIsEmpty(int id) => id == m_emptyIdValue;
    }
}
