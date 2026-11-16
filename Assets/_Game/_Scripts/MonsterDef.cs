using Game.Monsters.Combat;
using SegoLib;
using SegoLib.Random;
using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Monster Def", menuName = "Game/MonsterDef")]
    public class MonsterDef : ScriptableObject
    {
        [field: Tooltip("How skilled is this creature at combat.")]
        [field: SerializeField] public int CombatLevel { get; private set; } = 4;
        [field: Tooltip("How much damage the monster does on a landed attack. A weak creature would do 1, a strong creature would do 10+.")]
        [field: SerializeField] public float DamagePerAttack { get; private set; } = 4f;
        [field: Tooltip("How many seconds between attacks. Default is 60.")]
        [field: SerializeField] public float AttackCooldown { get; private set; } = 60f;
        [field: Tooltip("How much health the monster starts with. Default / a normal human is 100.")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    } 

    public class Monster : IFormattable
    {
        public MonsterDef Def { get; }
        public int CombatLevel => Def.CombatLevel;
        public float ExpReward { get; }
        public float CurrentHealth { get; set; }

        public Monster(MonsterDef def)
        {
            Def = def;
            CurrentHealth = def.MaxHealth;

            ExpReward = CalculateExperienceReward();
        }

        private float CalculateExperienceReward()
        {
            float expFromAttackDamage;

            if(Def.DamagePerAttack <= 0 || Def.AttackCooldown <= 0)
                expFromAttackDamage = 0;
            else
                expFromAttackDamage = (Def.DamagePerAttack / Def.AttackCooldown) * 60; // 1 point per damage per minute

            float expFromHealth = Def.MaxHealth * 0.1f; // 1 point per 10 health
            float expFromCombatLevel = CombatLevel; // 1 point per combat level
            return expFromAttackDamage + expFromHealth + expFromCombatLevel;
        }

        public float GetExpRewardModifiedByHealthLost()
        {
            return ExpReward * (1.0f - (CurrentHealth/Def.MaxHealth));
        }

        public override string ToString()
        {
            return $"{Def.name}";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
    }

    public class MonsterCombat : SegoLib.IDisposable
    {
        private const float OUT_OF_ENERGY_COOLDOWN_FACTOR = 1.5f;

        public WorldState WorldState { get; private set; }
        public Hero Hero { get; private set; }
        public Monster Monster { get; private set; }

        private float m_accumulatedHeroDamage;
        private float m_accumulatedMonsterDamage;

        public float ElapsedTime { get; private set; }
        public float NextHeroAttackTime { get; private set; }
        public float NextMonsterAttackTime { get; private set; }
        public bool HeroIsAttemptingToYield { get; private set; }

        public IMonsterCombatHeroYieldHandler ShouldHeroYield { get; private set; }

        public bool IsDisposed { get; private set; }
        public event Action<IPrivatelyDisposable> Disposed;

        public MonsterCombat(WorldState worldState, Hero hero, Monster monster, IMonsterCombatHeroYieldHandler shouldHeroYield)
        {
            WorldState = worldState;
            Hero = hero;
            Monster = monster;

            UpdateNextHeroAttackTime();
            UpdateNextMonsterAttackTime();
            this.ShouldHeroYield = shouldHeroYield;
        }

        private int GetHeroEffectiveCombatLevel()
        {
            return Hero.Equipment.GetMainWeaponSkill().Level;
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
            if (Monster.Def.DamagePerAttack <= 0 || Monster.Def.AttackCooldown <= 0)
                return 1;

            int heroLevel = GetHeroEffectiveCombatLevel();
            int monsterLevel = GetMonsterEffectiveCombatLevel();

            float heroHitRate = GetWinPercentForA(heroLevel, monsterLevel);
            float monsterHitRate = GetWinPercentForA(monsterLevel, heroLevel);

            float monsterDps = (Monster.Def.DamagePerAttack / Monster.Def.AttackCooldown) * monsterHitRate;
            float heroDps = (Hero.Equipment.MainWeapon.DamagePerAttack / Hero.Equipment.MainWeapon.AttackCooldown);

            if (Hero.Stamina.EnergyPercent <= float.Epsilon)
                heroDps /= OUT_OF_ENERGY_COOLDOWN_FACTOR;

            heroDps *= heroHitRate;

            float timeToKillMonster = Monster.CurrentHealth / heroDps;
            float timeToKillHero = Hero.Health.Current / monsterDps;

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

            Hero.Stamina.SpendEnergy(HeroStamina.COMBAT_ENERGY_PER_SECOND * deltaSeconds);

            while (!HeroIsAttemptingToYield && TryGetAttackAtCurrentTime(out Action attackAction)) {
                attackAction.Invoke();
            }

            ApplyAccumulatedDamage();
        }

        private bool TryGetAttackAtCurrentTime(out Action attackAction)
        {
            if(ElapsedTime >= NextHeroAttackTime && NextHeroAttackTime <= NextMonsterAttackTime) {
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

            HeroIsAttemptingToYield = ShouldHeroYield.ShouldHeroYield(this);

            if (HeroIsAttemptingToYield ||
                Hero.Health.Current - m_accumulatedHeroDamage <= 0 || 
                Hero.Stamina.ExhaustionPercent >= 1.0f)
                return;

            if(RollIsWon(GetHeroEffectiveCombatLevel(), GetMonsterEffectiveCombatLevel())) {
                m_accumulatedMonsterDamage += Hero.Equipment.MainWeapon.DamagePerAttack;
            }
        }
        private void DoMonsterAttack()
        {
            UpdateNextMonsterAttackTime();

            if (Monster.CurrentHealth - m_accumulatedMonsterDamage <= 0)
                return;

            if (RollIsWon(GetMonsterEffectiveCombatLevel(), GetHeroEffectiveCombatLevel())) {
                m_accumulatedHeroDamage += Monster.Def.DamagePerAttack;
            }
        }

        private bool RollIsWon(int attackerLevel, int defenderLevel)
        {
            int attackerRoll = HeroSkill.RollForContest(attackerLevel, WorldState.Rng);
            int defenderRoll = HeroSkill.RollForContest(defenderLevel, WorldState.Rng);

            return attackerRoll >= defenderRoll;
        }

        private void UpdateNextHeroAttackTime()
        {
            if(Hero.Equipment.MainWeapon.AttackCooldown <= 0) {
                NextHeroAttackTime = float.MaxValue;
                return;
            }

            if (Hero.Stamina.EnergyPercent <= float.Epsilon)
                NextHeroAttackTime += Hero.Equipment.MainWeapon.AttackCooldown * OUT_OF_ENERGY_COOLDOWN_FACTOR;
            else
                NextHeroAttackTime += Hero.Equipment.MainWeapon.AttackCooldown;
        }

        private void UpdateNextMonsterAttackTime()
        {
            if(Monster.Def.AttackCooldown <= 0)
                NextMonsterAttackTime = float.MaxValue;
            else
                NextMonsterAttackTime += Monster.Def.AttackCooldown;
        }

        private void ApplyAccumulatedDamage()
        {
            Monster.CurrentHealth -= m_accumulatedMonsterDamage;
            Hero.Health.Damage(m_accumulatedHeroDamage);

            m_accumulatedMonsterDamage = 0;
            m_accumulatedHeroDamage = 0;
        }

        public void Dispose()
        {
            if(IsDisposed) return;
            IsDisposed = true;

            WorldState = null;
            Hero = null;
            Monster = null;
            ShouldHeroYield = null;

            Disposed?.Invoke(this);
            Disposed = null;
        }
    }
}
