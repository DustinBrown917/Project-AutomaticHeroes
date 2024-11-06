

using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Hero Current Health Property", menuName = "Game/Hero Properties/Hero Current Health")]
    public class HeroProperty_CurrentHealth : HeroProperty<float>
    {
        public override float Get(Hero hero)
        {
            return hero.Health.Current;
        }

        public override void Set(Hero hero, float value)
        {
            hero.Health.Current = value;
        }
    } 
}
