

using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Hero Exhaustion Property", menuName = "Game/Hero Properties/Hero Exhaustion")]
    public class HeroProperty_Exhaustion : HeroProperty<float>
    {
        public override float Get(Hero hero)
        {
            return hero.Stamina.Exhaustion;
        }

        public override void Set(Hero hero, float value)
        {
            hero.Stamina.Exhaustion = value;
        }
    } 
}
