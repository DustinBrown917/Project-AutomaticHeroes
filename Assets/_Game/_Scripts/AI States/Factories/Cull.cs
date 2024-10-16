using SegoLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Cull : StateFactory<Cull.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {

            private float m_elapsed = 0f;

            private int m_desiredKillCount;
            private int m_currentKillCount;

            private FloatRange m_engagementFrequency = new FloatRange(300, 3600);

            private float m_nextEngagement;

            public State(Cull stateFactory, Sprite sprite, Hero hero, int desiredKillCount) : base(stateFactory, sprite, hero)
            {
                m_desiredKillCount = desiredKillCount;
                m_nextEngagement = WorldState.Instance.Rng.Range(m_engagementFrequency);
            }

            public override void DoState(float deltaSeconds)
            {
                m_elapsed += deltaSeconds;

                Hero.Stamina.TrySpend(deltaSeconds * HeroStamina.CULL_STAMINA_PER_SECOND);

                if(m_elapsed >= m_nextEngagement) {
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
                if (WorldState.Instance.Rng.Bool()) {
                    Hero.Health.Current -= WorldState.Instance.Rng.Range(1, 16);
                }

                Hero.Stamina.Current -= WorldState.Instance.Rng.Range(1, 6);

                Hero.Level.GainExperience(5);

                m_currentKillCount++;

                Progress = (float)m_currentKillCount / m_desiredKillCount;

                m_nextEngagement += WorldState.Instance.Rng.Range(m_engagementFrequency);
            }

            private bool TaskIsComplete()
            {
                return m_currentKillCount >= m_desiredKillCount ||
                    Hero.Stamina.Percent < 0.2f ||
                    Hero.Health.Percent < 0.2f;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, 10);
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("karate");
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.Combat * hero.Stamina.Percent * hero.Health.Percent;
        }
    } 
}
