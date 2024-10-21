using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Monster Def", menuName = "Game/MonsterDef")]
    public class MonsterDef : ScriptableObject
    {
        private const float EXP_PER_DAMAGE = 0.2f;
        private const float EXP_PER_WOUND = 0.5f;

        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public int Wounds { get; private set; }

        public float GetBonusExp()
        {
            return (Damage * EXP_PER_DAMAGE) + (Wounds * EXP_PER_WOUND);
        }
    } 

    public class Monster
    {
        public MonsterDef Def { get; }
        public int Level { get; }
        public int ExpReward { get; }
        public float CurrentWoundsRemaining { get; set; }

        public Monster(MonsterDef def, int level)
        {
            Def = def;
            Level = level;
            ExpReward = Mathf.RoundToInt(Level + def.GetBonusExp());

            CurrentWoundsRemaining = def.Wounds;
        }

        public float GetStaminaCostPerCombatRound(Hero hero)
        {
            return Level + ((((float)Level / hero.Skills[hero.Equipment.MainWeapon.WeaponType.Skill].Level) - 1) * Level);
        }

        public override string ToString()
        {
            return $"Lv. {Level} {Def.name}";
        }
    }

    public static class Combat
    {
        public readonly struct MonsterCombatResult
        {
            public readonly int heroRoll;
            public readonly bool heroHadEnoughEnergy;
            public readonly float damageTaken;
            public readonly int woundsDealt;

            public MonsterCombatResult(int heroRoll, bool heroHadEnoughEnergy, float damageTaken, int woundsDealt)
            {
                this.heroRoll = heroRoll;
                this.heroHadEnoughEnergy = heroHadEnoughEnergy;
                this.damageTaken = damageTaken;
                this.woundsDealt = woundsDealt;
            }
        }

        /// <summary>
        /// Do combat between a hero and a monster.<br/>
        /// Combat works like so:<br/>
        /// A roll is made to see who is the victor of this combat round. Roll = Rand(heroLevel [to] heroLevel + heroWeaponProficiency). <br/>
        /// If the roll meets or exceeds the monster's level, the hero deals a wound to the monster. <br/>
        /// If the roll is less than the monster's level, the monster deals damage to the hero. <br/>
        /// If the hero does not have sufficient energy to attack the monster (Monster.GetStaminaCostPerCombatRound), then they automatically roll 0.
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="monster"></param>
        /// <param name="worldState"></param>
        /// <returns></returns>
        public static MonsterCombatResult DoCombat(Hero hero, Monster monster, WorldState worldState)
        {
            int roll = 0;
            float damageTaken = 0;
            bool heroHasEnoughEnergy = hero.Stamina.TrySpendEnergy(monster.GetStaminaCostPerCombatRound(hero));
            int woundsDealt = 0;
            IWeapon heroWeapon = hero.Equipment.MainWeapon;

            if (heroHasEnoughEnergy) {
                int heroMaxRoll = hero.Skills[heroWeapon.WeaponType.Skill].Level * 2;
                roll = Mathf.RoundToInt(worldState.Rng.Float01() * heroMaxRoll) + heroWeapon.RollBonus;
            }

            if(roll >= monster.Level) {
                monster.CurrentWoundsRemaining--;
                woundsDealt = 1;
                hero.Skills[heroWeapon.WeaponType.Skill].GainExperience(2);

                if(monster.CurrentWoundsRemaining <= 0) {
                    hero.Skills[heroWeapon.WeaponType.Skill].GainExperience(monster.ExpReward);
                }

            } else {
                damageTaken = worldState.Rng.Float01() * monster.Def.Damage;
                hero.Health.Current -= damageTaken;
                hero.Skills[heroWeapon.WeaponType.Skill].GainExperience(1);
            }

            return new MonsterCombatResult(roll, heroHasEnoughEnergy, damageTaken, woundsDealt);
        }
    }
}
