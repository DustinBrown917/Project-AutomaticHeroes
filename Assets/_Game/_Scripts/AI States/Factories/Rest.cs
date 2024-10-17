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

            private float m_exhaustionRecoveredPerSecond = 25f / (8 * 3600);

            private float m_staminaRecoveredPerSecond;

            public State(Rest stateFactory, Sprite sprite, Hero hero, float staminaRecoveredPerHour) : base(stateFactory, sprite, hero)
            {
                m_staminaRecoveredPerSecond = staminaRecoveredPerHour / 3600;
            }

            public override void DoState(float deltaSeconds)
            {
                m_elapsed += deltaSeconds;

                Progress = Hero.Stamina.EnergyPercent;

                Hero.Stamina.Energy += deltaSeconds * m_staminaRecoveredPerSecond;
                Hero.Stamina.RecoverExhaustion(m_exhaustionRecoveredPerSecond * deltaSeconds);

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
                float chanceToWakeUp = GetTimeOfDayWakeUpModifier();

                if (WorldState.Instance.Rng.Bool(chanceToWakeUp)) {
                    CompleteState();
                }
            }

            private float GetTimeOfDayWakeUpModifier()
            {
                WorldDate.ETimeOfDay timeOfDay = WorldState.Instance.Date.TimeOfDay;

                switch (timeOfDay) {
                    case WorldDate.ETimeOfDay.Dawn:
                    case WorldDate.ETimeOfDay.Day:
                        if (Hero.Stamina.Exhaustion > 10) {
                            return 0.3f;
                        } else {
                            return 1.0f;
                        }
                    case WorldDate.ETimeOfDay.Dusk:
                    case WorldDate.ETimeOfDay.Night:
                    default:
                        if (Hero.Stamina.Exhaustion > 10) {
                            return 0.01f;
                        } else {
                            return 0.2f;
                        }
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
            //return hero.Brain.BasePriorities.StaminaRecovery * (1.0f - hero.Stamina.EnergyPercent) * RestQuality.GetPriorityImpact() * GetTimeOfDayPriorityModifier(); 

            return CalculatePriority(hero);
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

        private float CalculatePriority(Hero hero)
        {
            WorldDate.ETimeOfDay timeOfDay = WorldState.Instance.Date.TimeOfDay;
            switch (timeOfDay) {
                case WorldDate.ETimeOfDay.Dawn:
                case WorldDate.ETimeOfDay.Day:
                    if (hero.Stamina.Exhaustion < 25)
                        return 0;
                    else
                        return hero.Stamina.ExhaustionPercent;

                case WorldDate.ETimeOfDay.Dusk:
                    if (hero.Stamina.Exhaustion < 25)
                        return 0.4f;
                    else
                        return 1f;
                case WorldDate.ETimeOfDay.Night:
                default:
                    if (hero.Stamina.Exhaustion < 25)
                        return 1;
                    else
                        return 1.5f;
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
