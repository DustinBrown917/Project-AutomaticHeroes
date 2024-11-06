
using UnityEngine;

namespace Game
{
    public interface IHeroProperty
    {
        public object Get(Hero hero);
        public void Set(Hero hero, object value);
    }

    public interface IHeroProperty<T> : IHeroProperty
    {
        public new T Get(Hero hero);
        public void Set(Hero hero, T value);
    }

    public abstract class HeroProperty<T> : ScriptableObject, IHeroProperty<T>
    {
        public abstract T Get(Hero hero);
        object IHeroProperty.Get(Hero hero) => Get(hero);

        public abstract void Set(Hero hero, T value);

        public void Set(Hero hero, object value)
        {
            if(value is T typedValue)
                Set(hero, typedValue);
        }
    }
}