using SegoLib;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Monster Def", menuName = "Game/MonsterDef")]
    public class MonsterDef : ScriptableObject
    {
        [field: SerializeField] public float DamagePerHourPerLevel { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
    } 

    public class Monster
    {
        public MonsterDef Def { get; }
        public int Level { get; }
        public float ExpReward { get; }
        public float CurrentHealth { get; set; }
        public float DamagePerHour { get; }

        public Monster(MonsterDef def, int level)
        {
            Def = def;
            Level = level;

            CurrentHealth = def.MaxHealth;
            DamagePerHour = def.DamagePerHourPerLevel * level;
        }

        public float CalculateExperienceReward(Hero hero)
        {
            float heroSkillLevel = hero.Skills[hero.Equipment.MainWeapon.WeaponType.Skill].Level;
            return (Level * Def.DamagePerHourPerLevel * (Def.MaxHealth * 0.01f)) * (Level/heroSkillLevel);
        }

        public override string ToString()
        {
            return $"Lv. {Level} {Def.name}";
        }
    }

    public class MonsterCombat : SegoLib.IDisposable
    {
        public readonly struct StepResult
        {
            public readonly float damageToHero;
            public readonly float damageToMonster;
            public readonly float energySpent;

            public StepResult(float damageToHero, float damageToMonster, float energySpent)
            {
                this.damageToHero = damageToHero;
                this.damageToMonster = damageToMonster;
                this.energySpent = energySpent;
            }
        }

        public WorldState WorldState { get; private set; }
        public Hero Hero { get; private set; }
        public Monster Monster { get; private set; }

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> disposed;

        public MonsterCombat(WorldState worldState, Hero hero, Monster monster)
        {
            WorldState = worldState;
            Hero = hero;
            Monster = monster;
        }


        public void ApplyStepResult(in StepResult result)
        {
            Monster.CurrentHealth -= result.damageToMonster;
            Hero.Health.Damage(result.damageToHero);
            Hero.Stamina.SpendEnergy(result.energySpent);
        }

        public StepResult SimulateStep(float deltaSeconds)
        {
            float damageToMonster = WorldTime.PerHourToPerSecond(GetHeroDamagePerHour()) * deltaSeconds;
            float damageToHero = 0;

            if (damageToMonster < Monster.CurrentHealth) {
                damageToHero = WorldTime.PerHourToPerSecond(GetMonsterDamagePerHour()) * deltaSeconds;
            }

            float energySpent = HeroStamina.COMBAT_STAMINA_PER_SECOND * deltaSeconds;

            return new StepResult(damageToHero, damageToMonster, energySpent);
        }

        public void SimulateStepAndApply(float deltaSeconds)
        {
            ApplyStepResult(SimulateStep(deltaSeconds));
        }

        public float GetHeroDamagePerHour()
        {
            IWeapon heroWeapon = Hero.Equipment.MainWeapon;
            float damagePerHour = Hero.Skills[heroWeapon.WeaponType.Skill].Level; // Base skill level.
            damagePerHour += damagePerHour - Monster.Level; // Add level advantage/disadvantage
            damagePerHour += heroWeapon.UpgradeLevel; // Add weapon upgrade level
            damagePerHour *= 1.0f - Hero.Stamina.ExhaustionPercent; // Modify by exhaustion per cent.

            if (damagePerHour < 1.0f)
                damagePerHour = 1.0f;

            return damagePerHour;
        }

        public float GetMonsterDamagePerHour()
        {
            IWeapon heroWeapon = Hero.Equipment.MainWeapon;

            float damagePerHour = Monster.DamagePerHour;
            damagePerHour += Monster.Level - Hero.Skills[heroWeapon.WeaponType.Skill].Level;

            if (damagePerHour < Monster.Def.DamagePerHourPerLevel)
                damagePerHour = Monster.Def.DamagePerHourPerLevel;

            return damagePerHour;
        }

        public float GetTimeToHeroVictoryInSeconds()
        {
            float heroDamagePerHour = GetHeroDamagePerHour();
            return (Monster.CurrentHealth / heroDamagePerHour) * 3600f;
        }

        public float GetTimeToMonsterVictoryInSeconds()
        {
            float monsterDamagePerHour = GetMonsterDamagePerHour();
            return (Hero.Health.Current / monsterDamagePerHour) * 3600f;
        }

        public void Dispose()
        {
            if(IsDisposed) return;
            IsDisposed = true;

            WorldState = null;
            Hero = null;
            Monster = null;

            disposed?.Invoke(this);
            disposed = null;
        }
    }
}
