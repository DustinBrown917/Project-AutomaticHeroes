

using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Hero Current Health Percent Property", menuName = "Game/Hero Properties/Hero Current Health Percent")]
    public class HeroProperty_CurrentHealthPercent : HeroProperty<float>
    {
        public override float Get(Hero hero)
        {
            return hero.Health.Percent;
        }

        public override void Set(Hero hero, float value)
        {
            hero.Health.Current = value * hero.Health.Max;
        }
    } 
}
