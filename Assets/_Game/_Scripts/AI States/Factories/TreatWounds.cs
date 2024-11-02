using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class TreatWounds : HeroBehaviour
    {


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        public class Flavour
        {
            public string flavorText = string.Empty;
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== State ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.State
        {
            public float nextFlavourUpdate;

            public Flavour CurrentFlavour
            {
                get => m_flavour;
            }
            private Flavour m_flavour = DefaultFlavour;

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero) : base(behaviour, worldState, hero) { }

            public void SetFlavour(Flavour flavourState)
            {
                if (flavourState == null)
                    return;

                m_flavour = flavourState;
                FlavourText = m_flavour.flavorText;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End State ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Behaviour ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        protected class Behaviour : IHeroBehaviour.Behaviour<State>
        {
            public string stateString = "Treating Wounds.";

            public TreatmentQuality treatmentQuality;

            public List<Flavour> flavours = new();

            public Sprite sprite;

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour) { }

            protected override State CreateNewState(Hero hero)
            {
                State state = new State(ParentBehaviour, WorldState.Instance, hero);
                state.StateString = stateString;
                state.ForegroundSprite = sprite;
                return state;
            }
            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                float treatmentPerSecond = WorldTime.PerHourToPerSecond(treatmentQuality.TreatmentPerHour());
                bb.Hero.Health.Treated += treatmentPerSecond * deltaSeconds;

                bb.Hero.Stamina.SpendEnergy(HeroStamina.DEFAULT_STAMINA_PER_SECOND * deltaSeconds);

                if (bb.ElapsedTime >= bb.nextFlavourUpdate) {
                    bb.SetFlavour(GetNewFlavourState(bb));
                    bb.nextFlavourUpdate = bb.ElapsedTime + 4800f;
                }

                if (bb.Hero.Health.TreatedPercent >= 0.999f) {
                    bb.Succeed();
                }
            }

            private Flavour GetNewFlavourState(State bb)
            {
                if (flavours.Count > 0)
                    return bb.WorldState.Rng.GetRandom(flavours);
                else
                    return DefaultFlavour;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Behaviour ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        private static Flavour DefaultFlavour { get; } = new();

        public string StateString { get => m_behaviour.stateString; set => m_behaviour.stateString = value; }
        public TreatmentQuality TreatmentQuality { get => m_behaviour.treatmentQuality; set => m_behaviour.treatmentQuality = value; }
        public Sprite Sprite { get => m_behaviour.sprite; set => m_behaviour.sprite = value; }

        [SerializeField] private Behaviour m_behaviour;

        public TreatWounds()
        {
            m_behaviour = new Behaviour(this);
        }

        protected override IHeroBehaviour.IBehaviour InitializeBehaviour()
        {
            return m_behaviour;
        }

        public override bool CanHeroDoBehaviour(Hero hero)
        {
            return hero.Health.MissingHealth > hero.Health.Treated;
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
