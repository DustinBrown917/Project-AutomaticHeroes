using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Rest : StateFactory<Rest.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            private float m_elapsed = 0;

            private float m_staminaRecoveredPerSecond;

            public State(Rest stateFactory, Sprite sprite, Hero hero, float staminaRecoveredPerHour) : base(stateFactory, sprite, hero)
            {
                m_staminaRecoveredPerSecond = staminaRecoveredPerHour / 3600;
            }

            public override void DoState(float deltaSeconds)
            {
                m_elapsed += deltaSeconds;

                Progress = Hero.Stamina.Percent;

                Hero.Stamina.Current += deltaSeconds * m_staminaRecoveredPerSecond;

                if (m_elapsed > 3600) {
                    m_elapsed -= 3600;
                    CheckShouldComplete();
                }
            }

            public override void EnterState() { }

            public override void ExitState() { }

            public override string GetActionString() => "Resting.";

            public void CheckShouldComplete()
            {
                float chanceToWakeUp = Hero.Stamina.Percent * GetTimeOfDayWakeUpModifier();

                if (WorldState.Instance.Rng.Bool(chanceToWakeUp)) {
                    CompleteState();
                }
            }

            private float GetTimeOfDayWakeUpModifier()
            {
                WorldDate.ETimeOfDay timeOfDay = WorldState.Instance.Date.TimeOfDay;

                switch (timeOfDay) {
                    case WorldDate.ETimeOfDay.Night:
                        return 0.2f;
                    case WorldDate.ETimeOfDay.Dawn:
                        return 0.85f;
                    case WorldDate.ETimeOfDay.Day:
                        return 1.0f;
                    case WorldDate.ETimeOfDay.Dusk:
                    default:
                        return 0.5f;
                }
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [field: SerializeField] public RestQuality RestQuality { get; protected set; } = RestQuality.Decent;

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, RestQuality.StaminaRecoveredPerHour());
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("Rest Icon");
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.StaminaRecovery * (1.0f - hero.Stamina.Percent) * RestQuality.GetPriorityImpact() * GetTimeOfDayPriorityModifier(); 
        }

        protected virtual float GetTimeOfDayPriorityModifier()
        {
            WorldDate.ETimeOfDay timeOfDay = WorldState.Instance.Date.TimeOfDay;
            switch (timeOfDay) {
                case WorldDate.ETimeOfDay.Night:
                    return 1.5f;
                case WorldDate.ETimeOfDay.Dawn:
                case WorldDate.ETimeOfDay.Day:
                    return 1.0f;
                case WorldDate.ETimeOfDay.Dusk:
                default:
                    return 1.1f;
            }
        }
    } 

    public enum RestQuality : byte
    {
        Terrible,
        Mediocre,
        Decent,
        Good,
        Excellent
    }

    public static class RestQualityExtensions
    {
        public static float StaminaRecoveredPerHour(this RestQuality restQuality)
        {
            return ((int)restQuality * 2.5f) + 5;
        }

        public static float GetPriorityImpact(this RestQuality restQuality)
        {
            return 0.2f + ((int)restQuality * 0.2f);
        }
    }
}
