using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Travel : HeroBehaviour
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        protected class Flavour
        {
            public string text;
            public float energyCostImpact = 1.0f;
            public float speedImpact = 1.0f;
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

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero) : base(behaviour, worldState, hero)
            {
                ForegroundSprite = SpriteLibrary.GetSprite("Travel Icon");
            }

            public void SetFlavour(Flavour flavour)
            {
                if (flavour == null)
                    return;

                m_flavour = flavour;
                FlavourString = m_flavour.text;
            }

            public void OnRouteCompleted(HeroMovement heroMovement, IRouteTraversal route)
            {
                Hero.Movement.routeCompleted -= OnRouteCompleted;
                Succeed();
            }

            protected override void OnDisposed()
            {
                Hero.Movement.routeCompleted -= OnRouteCompleted;
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
            public IRouteTraversal traversal;

            public string stateString = "Travelling.";

            public List<Flavour> flavours = new();

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour) { }

            protected override State CreateNewState(Hero hero)
            {
                State state = new State(ParentBehaviour, WorldState.Instance, hero);
                state.StateString = stateString;
                return state;
            }

            protected override void OnBegin(State bb)
            {
                bb.Hero.Movement.CurrentRoute = traversal;
                bb.Hero.Movement.routeCompleted += bb.OnRouteCompleted;
                bb.Hero.Movement.Depart();
            }


            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                bb.Hero.Stamina.TrySpendEnergy(HeroStamina.MOVEMENT_STAMINA_PER_SECOND * bb.CurrentFlavour.energyCostImpact * deltaSeconds);

                bb.Hero.Movement.ProgressBySeconds(bb.CurrentFlavour.speedImpact * deltaSeconds);

                if(bb.ElapsedTime >= bb.nextFlavourUpdate) {
                    bb.SetFlavour(GetNewFlavour(bb));
                    bb.nextFlavourUpdate = bb.ElapsedTime + 4800f;
                }
            }

            private Flavour GetNewFlavour(State bb)
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

        private static Flavour DefaultFlavour { get; } = new() { text = string.Empty };

        private Behaviour m_behaviour;

        private List<Flavour> Flavours => m_behaviour.flavours;

        public Travel(IRouteTraversal routeTraversal)
        {
            m_behaviour = new Behaviour(this);
            m_behaviour.traversal = routeTraversal;
            m_behaviour.stateString = $"Travelling to {routeTraversal.End.DisplayName}";

            Flavours.Add(new Flavour() { text = "Marching steadily onward." });
            Flavours.Add(new Flavour() { text = "Moving at decent pace.", speedImpact = 1.05f, energyCostImpact = 1.05f });
            Flavours.Add(new Flavour() { text = "Moving hurriedly.", speedImpact = 1.1f, energyCostImpact = 1.1f});
            Flavours.Add(new Flavour() { text = "Taking their time.", speedImpact = 0.95f, energyCostImpact = 0.95f});
            Flavours.Add(new Flavour() { text = "Singing an old marching tune." });
            Flavours.Add(new Flavour() { text = "Whistling a song from their home." });
            Flavours.Add(new Flavour() { text = "Thinking intently as they walk." });
            Flavours.Add(new Flavour() { text = "Daydreaming absently." });
            Flavours.Add(new Flavour() { text = "Having a conversation with themelf." });
            Flavours.Add(new Flavour() { text = "Reflecting on past challenges." });
            Flavours.Add(new Flavour() { text = "Looking forward to taking a rest." });
            Flavours.Add(new Flavour() { text = "Anticipating their destination." });
            Flavours.Add(new Flavour() { text = "Trying to ignore their aching feet." });
            Flavours.Add(new Flavour() { text = "Thinking about what to eat." });
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
            float staminaCost = (m_behaviour.traversal.Length / hero.Movement.MoveSpeed) * HeroStamina.MOVEMENT_STAMINA_PER_HOUR;

            if (staminaCost > hero.Stamina.Energy)
                return 0;

            return CalculateBaseWeight() * hero.Brain.BasePriorities.RandomTravel;
        }

        private float CalculateBaseWeight()
        {
            return 1.0f / m_behaviour.traversal.Start.Routes.Count;
        }
    }
}
