using System;
using System.Linq;


namespace SegoLib
{
    /// <summary>
    /// A storage object to manage objects that should be uniquely identifiable.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public abstract class ObjectLibrary_Identity<TObject> : ObjectLibrary<Identity, TObject> where TObject : class, IIdentifiable<Identity>
    {
        /// <summary>
        /// The characters that can be used for generated Ids.
        /// </summary>
        private static readonly char[] chars = new char[] { 'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e',
        'F', 'f', 'G', 'g', 'H', 'h', 'I', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o',
        'P', 'p', 'Q', 'q', 'R', 'r', 'S', 's', 'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y',
        'Z', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        /// <summary>
        /// If no identity builder is provided, this will be used.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static Identity DefaultIdentityBuilder(string id)
        {
            return Identity<TObject>.Get(id);
        }

        /// <summary>
        /// Used to get a new identity given a string.
        /// </summary>
        private Func<string, Identity> m_identityBuilder;

        /// <summary>
        /// Default Ctor.
        /// </summary>
        public ObjectLibrary_Identity() : this(DefaultIdentityBuilder) { }

        /// <summary>
        /// Ctor.
        /// </summary>
        public ObjectLibrary_Identity(Func<string, Identity> identityBuilder) : base() 
        {
            if (identityBuilder == null)
                m_identityBuilder = DefaultIdentityBuilder;
            else
                m_identityBuilder = identityBuilder;
        }


        /// <inheritdoc/>
        protected override void UpdateNextId()
        {
            while (m_nextId == null || m_library.ContainsKey(m_nextId)) m_nextId = m_identityBuilder(GetRandomString());
        }

        /// <inheritdoc/>
        public override bool IdIsEmpty(Identity id)
        {
            return id == null;
        }

        private string GetRandomString() => new string(Enumerable.Repeat(chars, 12).Select(s => chars.Random()).ToArray());
    }
}
