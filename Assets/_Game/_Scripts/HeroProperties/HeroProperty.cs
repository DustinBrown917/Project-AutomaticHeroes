
using UnityEngine;

namespace Game
{
    public abstract class HeroProperty : ScriptableObject
    {
        public abstract object GetBoxed(Hero hero);

        public abstract void SetBoxed(Hero hero, object value);
    }

    public abstract class HeroProperty<T> : HeroProperty
    {
        public abstract T Get(Hero hero);
        public override object GetBoxed(Hero hero) => Get(hero);

        public abstract void Set(Hero hero, T value);
        public override void SetBoxed(Hero hero, object value)
        {
            if(value is T typedValue)
                Set(hero, typedValue);
        }
    }
}