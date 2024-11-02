using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Rest : HeroBehaviour
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        public class Flavour
        {
            public string flavorText;
            public float exhaustionRecoveryImpact = 1.0f;
            public float energyRecoveryImpact = 1.0f;
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== State ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.State
        {
            public readonly float exhaustionRecoveredPerSecond = 25f / (8 * 3600);

            public float energyRecoveredPerSecond = 100f / (8 * 3600);

            public float nextCheckCompleteTime;

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
            public RestQuality restQuality;

            public List<Flavour> flavours = new();

            public string stateString = "Resting.";

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
                bb.Hero.Stamina.Energy += bb.energyRecoveredPerSecond * restQuality.BenefitMultiplier() * bb.CurrentFlavour.energyRecoveryImpact * deltaSeconds;
                bb.Hero.Stamina.RecoverExhaustion(bb.exhaustionRecoveredPerSecond * restQuality.BenefitMultiplier() * bb.CurrentFlavour.exhaustionRecoveryImpact * deltaSeconds);

                if (bb.ElapsedTime >= bb.nextCheckCompleteTime) {
                    CheckComplete(bb);
                    bb.nextCheckCompleteTime = bb.ElapsedTime + 3600f;
                }

                if(bb.ElapsedTime >= bb.nextFlavourUpdate) {
                    bb.SetFlavour(GetNewFlavourState(bb));
                    bb.nextFlavourUpdate = bb.ElapsedTime + 4800f;
                }
            }

            private void CheckComplete(State bb)
            {
                float chanceToWakeUp = GetTimeOfDayWakeUpModifier(bb);

                if (bb.WorldState.Rng.Bool(chanceToWakeUp)) {
                    bb.Succeed();
                }
            }

            private float GetTimeOfDayWakeUpModifier(State bb)
            {
                WorldDate.ETimeOfDay timeOfDay = bb.WorldState.Date.TimeOfDay;

                switch (timeOfDay) {
                    case WorldDate.ETimeOfDay.Dawn:
                    case WorldDate.ETimeOfDay.Day:
                        if (bb.Hero.Stamina.Exhaustion > 10) {
                            return 0.3f;
                        } else {
                            return 1.0f;
                        }
                    case WorldDate.ETimeOfDay.Dusk:
                    case WorldDate.ETimeOfDay.Night:
                    default:
                        if (bb.Hero.Stamina.Exhaustion > 10) {
                            return 0.01f;
                        } else {
                            return 0.2f;
                        }
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

        private static Flavour DefaultFlavour { get; } = new() { flavorText = string.Empty };

        [SerializeField] private Behaviour m_behaviour;

        public List<Flavour> Flavours => m_behaviour.flavours;
        public RestQuality RestQuality { get => m_behaviour.restQuality; set => m_behaviour.restQuality = value; }
        public string StateString { get => m_behaviour.stateString; set => m_behaviour.stateString = value; }
        public Sprite Sprite { get => m_behaviour.sprite; set => m_behaviour.sprite = value; }

        public Rest()
        {
            m_behaviour = new Behaviour(this);
        }

        protected override IHeroBehaviour.IBehaviour InitializeBehaviour()
        {
            Flavours.Add(new Flavour() { flavorText = "Sleeping soundly.", energyRecoveryImpact = 1.05f, exhaustionRecoveryImpact = 1.05f });
            Flavours.Add(new Flavour() { flavorText = "Having a restful sleep.", energyRecoveryImpact = 1.05f, exhaustionRecoveryImpact = 1.05f });
            Flavours.Add(new Flavour() { flavorText = "Tossing and turning.", energyRecoveryImpact = 0.95f, exhaustionRecoveryImpact = 0.95f });
            Flavours.Add(new Flavour() { flavorText = "Having trouble falling asleep", energyRecoveryImpact = 0.95f, exhaustionRecoveryImpact = 0.95f });
            Flavours.Add(new Flavour() { flavorText = "Having a peaceful dream." });
            Flavours.Add(new Flavour() { flavorText = "Snoring loudly." });
            Flavours.Add(new Flavour() { flavorText = "Having a fitful dream." });
            Flavours.Add(new Flavour() { flavorText = "Dreaming of a delicious meal." });
            Flavours.Add(new Flavour() { flavorText = "Having a nonsensical dream." });
            Flavours.Add(new Flavour() { flavorText = "Dreaming of a lost love." });
            Flavours.Add(new Flavour() { flavorText = "Dreaming of a deep regret." });
            Flavours.Add(new Flavour() { flavorText = "Dreaming of a better time." });
            Flavours.Add(new Flavour() { flavorText = "Dreaming of the road ahead." });
            Flavours.Add(new Flavour() { flavorText = "Dreaming about waterfalls." });

            return m_behaviour;
        }

        public override bool CanHeroDoBehaviour(Hero hero) => true;

        public override float GetPriority(Hero hero) => CalculatePriority(hero) * RestQuality.GetPriorityImpact();

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
        public static float BenefitMultiplier(this RestQuality restQuality)
        {
            return 0.5f + ((int)restQuality * 0.25f);
        }

        public static float StaminaRecoveredPerHour(this RestQuality restQuality)
        {
            return ((int)restQuality * 2.5f) + 5;
        }

        public static float GetPriorityImpact(this RestQuality restQuality)
        {
            return ((restQuality.BenefitMultiplier() - 1) * 0.8f) + 1;
        }
    }
}
