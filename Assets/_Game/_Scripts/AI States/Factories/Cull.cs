using SegoLib;
using SegoLib.Random;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Cull : StateFactory<Cull.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        public class MonsterPopulation
        {
            [field: SerializeField] public MonsterDef Monster { get; private set; }
            [field: SerializeField] public IntRange Level { get; private set; }
            [field: Tooltip("How many of this monster one is likely to run into in an hour.")]
            [field: SerializeField] public float Population { get; private set; }
        }

        public class State : State_Base
        {
            private StringBuilder m_stringBuilder = new StringBuilder();

            private MonsterPopulation[] m_monsterPopulations;
            private float m_sumOfPopulation = 0;

            private float m_elapsed = 0f;

            private int m_desiredKillCount;
            private int m_currentKillCount;

            private float m_nextEngagementTime;

            public State(Cull stateFactory, Sprite sprite, Hero hero, MonsterPopulation[] monsterPopulations, int desiredKillCount) : base(stateFactory, sprite, hero)
            {
                m_desiredKillCount = desiredKillCount;
                m_monsterPopulations = monsterPopulations;
                m_nextEngagementTime = GetNextEngagementTime();

                foreach(MonsterPopulation population in monsterPopulations) {
                    m_sumOfPopulation += population.Population;
                }
            }

            private float GetNextEngagementTime()
            {
                return m_elapsed + (WorldState.Instance.Rng.Range(m_sumOfPopulation * 0.85f, m_sumOfPopulation * 1.15f) * 3600);
            }

            private Monster GetNextEngagement()
            {
                int index = WeightedRandom.Naive(m_monsterPopulations, m_sumOfPopulation, WorldState.Instance.Rng, (x) => x.Population);
                return new Monster(m_monsterPopulations[index].Monster, WorldState.Instance.Rng.Range(m_monsterPopulations[index].Level));
            }

            public override void DoState(float deltaSeconds)
            {
                m_elapsed += deltaSeconds;

                Hero.Stamina.TrySpendEnergy(deltaSeconds * HeroStamina.CULL_STAMINA_PER_SECOND);

                if(m_elapsed >= m_nextEngagementTime) {
                    DoEngagement();
                }

                if (TaskIsComplete()) {
                    CompleteState();
                }
            }

            public override void EnterState() { }

            public override void ExitState() { }

            public override string GetActionString() => "Culling creatures.";

            private void DoEngagement()
            {
                Monster monster = GetNextEngagement();

                bool couldSpendEnergy = Hero.Stamina.TrySpendEnergy(10.0f * ((float)monster.Level / Hero.Level.Level));

                int monsterRoll = WorldState.Instance.Rng.Range(0, monster.Level + 1);
                int heroRoll = WorldState.Instance.Rng.Range(0, Hero.Level.Level + 1);

                int damageReceived = 0;
                int expGained = 0;

                if(monsterRoll > heroRoll) {
                    damageReceived = Mathf.RoundToInt(WorldState.Instance.Rng.Float01() * monster.AttackDamage);
                    Hero.Health.Current -= damageReceived;
                } 

                if (couldSpendEnergy) {

                    expGained = monster.ExpReward;

                    Hero.Level.GainExperience(expGained);

                    m_currentKillCount++;

                    Progress = (float)m_currentKillCount / m_desiredKillCount;
                }


                m_stringBuilder.Append(Hero.Color.ColoredName);
                if(couldSpendEnergy)
                    m_stringBuilder.Append(" defeated ");
                else
                    m_stringBuilder.Append(" encountered ");
                m_stringBuilder.Append("a level ");
                m_stringBuilder.Append(monster.Level);
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(monster.Def.name);
                if(damageReceived > 0) {
                    m_stringBuilder.Append(", taking ");
                    m_stringBuilder.Append(damageReceived);
                    m_stringBuilder.Append(" damage");
                }

                if (couldSpendEnergy) {
                    if (damageReceived > 0)
                        m_stringBuilder.Append(" and gaining ");
                    else
                        m_stringBuilder.Append(", gaining ");
                    m_stringBuilder.Append(expGained);
                    m_stringBuilder.Append(" exp.");
                } else {
                    m_stringBuilder.Append(" but could not defeat it.");
                }

                ActionFeed.Post(m_stringBuilder.ToString());
                m_stringBuilder.Clear();
                m_nextEngagementTime = GetNextEngagementTime();
            }

            private bool TaskIsComplete()
            {
                return m_currentKillCount >= m_desiredKillCount ||
                    Hero.Stamina.EnergyPercent < 0.2f ||
                    Hero.Stamina.ExhaustionPercent > 0.5f ||
                    Hero.Health.Percent < 0.2f;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private MonsterPopulation[] m_monsterPopulations;

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, m_monsterPopulations, 10);
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("karate");
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.Combat * hero.Stamina.EnergyPercent * hero.Health.Percent;
        }
    } 
}
