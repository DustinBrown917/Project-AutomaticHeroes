using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Training : HeroBehaviour
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        protected class Flavour
        {
            public string flavorText;
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
                FlavourString = m_flavour.flavorText;
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
            public List<Flavour> flavours = new();

            public string stateString = "Training.";

            public Sprite sprite;

            private float m_trainingDuration = 4 * 3600;
            private float m_expPerSecond = WorldTime.PerHourToPerSecond(1.5f);

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour) { }

            protected override State CreateNewState(Hero hero)
            {
                State state = new State(ParentBehaviour, WorldState.Instance, hero);
                state.StateString = stateString;
                state.Sprite = sprite;
                return state;
            }


            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                if (!bb.Hero.Stamina.TrySpendEnergy(deltaSeconds * HeroStamina.TRAINING_STAMINA_PER_SECOND)) {
                    bb.Succeed();
                } else {
                    bb.Hero.Level.GainExperience(deltaSeconds * m_expPerSecond);

                    if (bb.ElapsedTime >= bb.nextFlavourUpdate) {
                        bb.SetFlavour(GetNewFlavourState(bb));
                        bb.nextFlavourUpdate = bb.ElapsedTime + 4800f;
                    }
                }

                if (bb.ElapsedTime >= m_trainingDuration)
                    bb.Succeed();
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
        public string StateString { get => m_behaviour.stateString; set => m_behaviour.stateString = value; }
        public Sprite Sprite { get => m_behaviour.sprite; set => m_behaviour.sprite = value; }

        public Training()
        {
            m_behaviour = new Behaviour(this);
        }

        protected override IHeroBehaviour.IBehaviour InitializeBehaviour()
        {
            return m_behaviour;
        }

        public override bool CanHeroDoBehaviour(Hero hero)
        {
            return true;
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.Training * hero.Stamina.EnergyPercent;
        }
    }
}
