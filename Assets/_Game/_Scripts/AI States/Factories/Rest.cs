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
            public float duration = 0;
            public float elapsed = 0;

            private float m_staminaRecoveredPerSecond;

            public State(Rest stateFactory, Hero hero, float staminaRecoveredPerHour) : base(stateFactory, hero)
            {
                m_staminaRecoveredPerSecond = staminaRecoveredPerHour / 3600;
                duration = (hero.Stamina.Max - hero.Stamina.Current) / m_staminaRecoveredPerSecond;
            }

            public override void DoState(float deltaSeconds)
            {
                elapsed += deltaSeconds;

                Progress = elapsed / duration;

                Hero.Stamina.Current += deltaSeconds * m_staminaRecoveredPerSecond;

                if (elapsed > duration) {
                    CompleteState();
                }
            }

            public override void EnterState() { }

            public override void ExitState() { }

            public override string GetActionString() => "Resting.";
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [field: SerializeField] public RestQuality RestQuality { get; private set; } = RestQuality.Decent;

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, hero, RestQuality.StaminaRecoveredPerHour());
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("Rest Icon");
        }

        public override float GetPriority(Hero hero)
        {
            return (1.0f - hero.Stamina.Percent) * RestQuality.GetPriorityImpact(); 
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
