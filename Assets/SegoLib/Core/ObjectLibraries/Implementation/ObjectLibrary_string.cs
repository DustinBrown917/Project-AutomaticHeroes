using System.Linq;


namespace SegoLib
{
    /// <summary>
    /// A storage object to manage objects that should be uniquely identifiable.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public abstract class ObjectLibrary_string<TObject> : ObjectLibrary<string, TObject> where TObject : class, IIdentifiable<string>
    {
        /// <summary>
        /// The characters that can be used for generated Ids.
        /// </summary>
        private static readonly char[] chars = new char[] { 'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e',
        'F', 'f', 'G', 'g', 'H', 'h', 'I', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o',
        'P', 'p', 'Q', 'q', 'R', 'r', 'S', 's', 'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y',
        'Z', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObjectLibrary_string() : base() { }


        /// <inheritdoc/>
        protected override void UpdateNextId()
        {
            while (string.IsNullOrEmpty(m_nextId) || m_library.ContainsKey(m_nextId)) m_nextId = GetRandomString();
        }

        /// <inheritdoc/>
        public override bool IdIsEmpty(string id) => string.IsNullOrWhiteSpace(id);

        private string GetRandomString() => new string(Enumerable.Repeat(chars, 12).Select(s => chars.Random()).ToArray());
    }
}
