

using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Hero Exhaustion Percent Property", menuName = "Game/Hero Properties/Hero Exhaustion Percent")]
    public class HeroProperty_ExhaustionPercent : HeroProperty<float>
    {
        public override float Get(Hero hero)
        {
            return hero.Stamina.ExhaustionPercent;
        }

        public override void Set(Hero hero, float value)
        {
            hero.Stamina.Exhaustion = value * hero.Stamina.MaxEnergy;
        }
    } 
}
