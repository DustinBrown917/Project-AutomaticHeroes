using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class TreatWounds : StateFactory<TreatWounds.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            public float duration = 0;
            public float elapsed = 0;


            private float m_treatmentPerSecond;

            public State(TreatWounds stateFactory, Sprite sprite, Hero hero, float treatmentPerHour) : base(stateFactory, sprite, hero)
            {
                m_treatmentPerSecond = WorldTime.PerHourToPerSecond(treatmentPerHour);
            }

            public override void DoState(float deltaSeconds)
            {
                elapsed += deltaSeconds;

                Hero.Health.Treated += m_treatmentPerSecond * deltaSeconds;

                Progress = Hero.Health.TreatedPercent;

                Hero.Stamina.TrySpend(HeroStamina.DEFAULT_STAMINA_PER_SECOND * deltaSeconds);

                if (Hero.Health.TreatedPercent >= 0.999f) {
                    CompleteState();
                }
            }

            public override void EnterState() { }

            public override void ExitState() { }

            public override string GetActionString() => "Treating Wounds.";
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [field: SerializeField] public TreatmentQuality TreatmentQuality { get; private set; } = TreatmentQuality.Decent;

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, TreatmentQuality.TreatmentPerHour());
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("firstaid");
        }

        public override float GetPriority(Hero hero)
        {
            if (hero.Health.NeedsTreatment()) {
                float priority = hero.Brain.BasePriorities.WoundTreatment * (1.0f - hero.Health.TreatedPercent) * TreatmentQuality.GetPriorityImpact();
                return priority;
            }

            return 0f;
        }
    }

    public enum TreatmentQuality : byte
    {
        Terrible,
        Mediocre,
        Decent,
        Good,
        Excellent
    }

    public static class TreatmentQualityExtensions
    {
        public static float TreatmentPerHour(this TreatmentQuality restQuality)
        {
            return ((int)restQuality * 2.5f) + 5;
        }

        public static float GetPriorityImpact(this TreatmentQuality restQuality)
        {
            return 0.2f + ((int)restQuality * 0.2f);
        }
    }
}
