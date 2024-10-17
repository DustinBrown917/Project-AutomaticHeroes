using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Training_General : StateFactory<Training_General.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {


            public float durationInSeconds = 0;
            public float elapsed = 0;

            private float m_expPerSecond;

            public State(Training_General stateFactory, Sprite sprite, Hero hero, float expPerHour) : base(stateFactory, sprite, hero)
            {
                m_expPerSecond = expPerHour / 3600;

                float staminaHours = hero.Stamina.Energy / HeroStamina.TRAINING_STAMINA_PER_HOUR;

                float hours = Mathf.Min((int)staminaHours, 4);
                durationInSeconds = hours * 3600;
            }

            public override void DoState(float deltaSeconds)
            {
                elapsed += deltaSeconds;

                Hero.Stamina.TrySpendEnergy(deltaSeconds * HeroStamina.TRAINING_STAMINA_PER_SECOND);

                Progress = elapsed / durationInSeconds;

                Hero.Level.GainExperience(deltaSeconds * m_expPerSecond);

                if(elapsed > durationInSeconds) {
                    CompleteState();
                }
            }

            public override void EnterState() { }

            public override void ExitState() { }

            public override string GetActionString() => "Training.";
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [field: SerializeField] public TrainingQuality TrainingQuality { get; private set; } = TrainingQuality.Decent;

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, TrainingQuality.ExpPerHour());
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("Training Icon");
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.Training * TrainingQuality.GetPriorityImpact() * hero.Stamina.EnergyPercent;
        }
    } 

    public enum TrainingQuality
    {
        Terrible,
        Mediocre,
        Decent,
        Good,
        Excellent
    }

    public static class TrainingQualityExtensions
    {
        public static float ExpPerHour(this TrainingQuality quality)
        {
            return ((int)quality * 0.5f) + 0.5f;
        }

        public static float GetPriorityImpact(this TrainingQuality quality)
        {
            return 0.2f + ((int)quality * 0.2f);
        }
    }
}
