using SegoLib;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Monster Def", menuName = "Game/MonsterDef")]
    public class MonsterDef : ScriptableObject
    {
        [field: SerializeField] public int CombatLevel { get; private set; }
        [field: SerializeField] public float DamagePerAttack { get; private set; }
        [field: SerializeField] public float AttackCooldown { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
    } 

    public class Monster
    {
        public MonsterDef Def { get; }
        public int CombatLevel => Def.CombatLevel;
        public float ExpReward { get; }
        public float CurrentHealth { get; set; }

        public Monster(MonsterDef def)
        {
            Def = def;
            CurrentHealth = def.MaxHealth;
        }

        public float CalculateExperienceReward(Hero hero)
        {
            float heroSkillLevel = hero.Skills[hero.Equipment.MainWeapon.WeaponType.Skill].Level;
            float exp = (CombatLevel + (CombatLevel - heroSkillLevel)) * Combat.EXP_PER_MONSTER_LEVEL;
            exp *= (Def.DamagePerAttack * 0.01f) * (Def.MaxHealth * 0.01f);
            if (exp < 1)
                exp = 1;
            return exp;
        }

        public override string ToString()
        {
            return $"{Def.name}";
        }
    }

    public static class Combat
    {
        public const float EXP_PER_MONSTER_LEVEL = 3f;
        public const float HERO_DAMAGE_PER_HOUR_AT_EQUAL_LEVEL = 100f;
        public const float LEVEL_DIFFERENCE_DAMAGE_INFLUENCE = 1.0f;
    }

    public class MonsterCombat : SegoLib.IDisposable
    {
        public const float HERO_SECONDS_PER_ATTACK = 60f;
        public const float HERO_DAMAGE_PER_ATTACK = 2.0f;

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

        private float m_accumulatedHeroDamage;
        private float m_accumulatedMonsterDamage;

        public float ElapsedTime { get; private set; }
        public float NextHeroAttackTime { get; private set; }
        public float NextMonsterAttackTime { get; private set; }

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> disposed;

        public MonsterCombat(WorldState worldState, Hero hero, Monster monster)
        {
            WorldState = worldState;
            Hero = hero;
            Monster = monster;

            UpdateNextHeroAttackTime();
            UpdateNextMonsterAttackTime();
        }

        private int GetHeroEffectiveCombatLevel()
        {
            return Hero.Equipment.GetMainWeaponSkill().Level + Hero.Equipment.MainWeapon.UpgradeLevel;
        }

        private int GetMonsterEffectiveCombatLevel()
        {
            return Monster.CombatLevel;
        }

        /// <summary>
        /// Get a value between -1 and 1 indicating the balance of power in this fight, where -1 is overwhelmingly in favour of the monster, 1 is overwhelmingly in favour of the hero, and 0 is approximately equal.
        /// </summary>
        /// <returns></returns>
        public float GetBalanceOfPower()
        {
            int heroLevel = GetHeroEffectiveCombatLevel();
            int monsterLevel = GetMonsterEffectiveCombatLevel();

            float heroHitRate = GetWinPercentForA(heroLevel, monsterLevel);
            float monsterHitRate = GetWinPercentForA(monsterLevel, heroLevel);

            float monsterDps = (Monster.Def.DamagePerAttack / Monster.Def.AttackCooldown) * monsterHitRate;
            float heroDps = (HERO_DAMAGE_PER_ATTACK / HERO_SECONDS_PER_ATTACK) * heroHitRate;

            float timeToKillMonster = Monster.CurrentHealth / heroDps;
            float timeToKillHero = Hero.Health.Current / monsterDps;
            
            // TODO: Consider hero exhaustion in this calculation.

            float sum = timeToKillMonster + timeToKillHero;

            float balance = ((timeToKillHero / sum) * 2) - 1;

            return balance;
        }

        /// <summary>
        /// Given uniformly distributed rolls, gets the chance tha a roll on a <paramref name="maxA"/> sided dice 
        /// would be greater than or equal to a roll on a <paramref name="maxB"/> sided dice. <br/>
        /// IMPORTANT: This assumes that a draw is a win for <paramref name="maxA"/>.
        /// </summary>
        /// <param name="maxA"></param>
        /// <param name="maxB"></param>
        /// <returns></returns>
        public float GetWinPercentForA(int maxA, int maxB)
        {
            // Increment both to account for 0 being a valid roll value.
            maxA++;
            maxB++;

            // Get total possible outcomes.
            int totalOutcomes = maxA * maxB;

            // Get the lower of the two maxes.
            int lowerMax = Mathf.Min(maxA, maxB);

            // If maxA == maxB, win rate would be calculated as (n(n + 1))/2 (see nth triangle number. Look it up, nerd.)
            int wins = (lowerMax * (lowerMax + 1)) / 2;

            // If maxA > maxB, then for every digit extra maxA has over maxB, add maxB to favourable outcomes.
            // This is because if maxA rolls one of these numbers, no matter what maxB rolls, maxA will win.
            if (maxA > maxB)
                wins += (maxA - maxB) * maxB;

            // Get proportion of total outcomes that are wins for maxA.
            return (float)wins / totalOutcomes;
        }


        public void Simulate(float deltaSeconds)
        {
            ElapsedTime += deltaSeconds;

            Hero.Stamina.SpendEnergy(HeroStamina.COMBAT_STAMINA_PER_SECOND * deltaSeconds);

            while (TryGetAttackAtCurrentTime(out Action attackAction)) {
                attackAction.Invoke();
            }

            ApplyAccumulatedDamage();
        }

        private bool TryGetAttackAtCurrentTime(out Action attackAction)
        {
            if(ElapsedTime >= NextHeroAttackTime) {
                attackAction = DoHeroAttack;
            } else if(ElapsedTime >= NextMonsterAttackTime) {
                attackAction = DoMonsterAttack;
            } else {
                attackAction = null;
            }

            return attackAction != null;
        }

        private void DoHeroAttack()
        {
            UpdateNextHeroAttackTime();

            if (Hero.Health.Current - m_accumulatedHeroDamage <= 0 || Hero.Stamina.ExhaustionPercent >= 1.0f)
                return;

            if(RollIsWon(GetHeroEffectiveCombatLevel(), GetMonsterEffectiveCombatLevel())) {
                m_accumulatedMonsterDamage += HERO_DAMAGE_PER_ATTACK;
            }
        }
        private void DoMonsterAttack()
        {
            UpdateNextMonsterAttackTime();

            if (Monster.CurrentHealth - m_accumulatedMonsterDamage <= 0)
                return;

            if (RollIsWon(GetMonsterEffectiveCombatLevel(), GetHeroEffectiveCombatLevel())) {
                m_accumulatedMonsterDamage += Monster.Def.DamagePerAttack;
            }
        }

        private bool RollIsWon(int attackerLevel, int defenderLevel)
        {
            int attackerRoll = WorldState.Rng.Range(0, attackerLevel + 1);
            int defenderRoll = WorldState.Rng.Range(0, defenderLevel + 1);

            return attackerRoll > defenderRoll;
        }

        private void UpdateNextHeroAttackTime()
        {
            NextHeroAttackTime += HERO_SECONDS_PER_ATTACK;
        }

        private void UpdateNextMonsterAttackTime()
        {
            NextMonsterAttackTime += Monster.Def.AttackCooldown;
        }

        private void ApplyAccumulatedDamage()
        {
            Monster.CurrentHealth -= m_accumulatedMonsterDamage;
            Hero.Health.Damage(m_accumulatedHeroDamage);
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
