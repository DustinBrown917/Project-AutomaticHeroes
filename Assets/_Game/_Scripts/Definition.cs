using SegoLib;
using UnityEngine;

namespace Game
{
    public interface IDefinition : IIdentifiable<Identity>, IDisplayName, IDescription, IDisplaySprite
    {
        public string Label { get; }
        public string SpriteAddress { get; }
        public int RuntimeIndex { get; }
    }






    public abstract class Definition : IDefinition
    {
        public Identity Id { get; }
        public int RuntimeIndex => m_runtimeIndex;
        private readonly int m_runtimeIndex;
        public abstract string Label { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string SpriteAddress { get; }
        public Sprite DisplaySprite { get; }

        protected Definition(Identity identity, string displayName, string description) : this(identity, displayName, string.Empty, description) { }

        protected Definition(Identity identity, string displayName, string spriteAddress, string description)
        {
            Id = identity;
            DisplayName = displayName;
            SpriteAddress = spriteAddress;
            Description = description;
            Register(ref m_runtimeIndex);
            //DisplaySprite = SpriteLibrary.GetSprite(SpriteAddress);
        }

        protected abstract void Register(ref int runtimeIndex);

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString() => Id.ToString();
    }



    public interface IDefinition<T> : IDefinition where T : IDefinition<T> { }



    public abstract class Definition<T> : Definition, IDefinition<T> where T : Definition<T>
    {
        protected Definition(string identity, string displayName, string description) : this(identity, displayName, string.Empty, description) { }

        protected Definition(string identity, string displayName, string spriteAddress, string description) : base(Identity<T>.GetNew(identity), displayName, spriteAddress, description)
        {
        }

        protected override void Register(ref int runtimeIndex)
        {
            All<T>.Register((T)this, out runtimeIndex);
        }
    }
}
