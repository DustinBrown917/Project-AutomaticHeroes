using SegoLib.Random;
using UnityEngine;

namespace Game
{
    public class Travel : StateFactory<Travel.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            private IRouteTraversal m_traversal;

            public State(Travel stateFactory, Hero hero, Sprite sprite, IRouteTraversal traversal) : base(stateFactory, sprite, hero)
            {
                m_traversal = traversal;
            }

            private void Movement_RouteCompleted(HeroMovement heroMovement, IRouteTraversal route)
            {
                Hero.Movement.routeCompleted -= Movement_RouteCompleted;
                CompleteState();
            }

            public override void EnterState()
            {
                Hero.Movement.CurrentRoute = m_traversal;
                Hero.Movement.routeCompleted += Movement_RouteCompleted;
                Hero.Movement.Depart();
            }

            public override void DoState(float deltaSeconds)
            {
                Hero.Stamina.TrySpend(HeroStamina.MOVEMENT_STAMINA_PER_SECOND * deltaSeconds);

                Hero.Movement.ProgressBySeconds(deltaSeconds);
                Progress = Hero.Movement.RouteProgress;
            }

            public override void ExitState()
            {
                Hero.Movement.routeCompleted -= Movement_RouteCompleted;
            }

            public override string GetActionString() => $"Travelling to {Hero.Movement.CurrentRoute.End.DisplayName}.";
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        public IRouteTraversal RouteTraversal { get; }

        public Travel(IRouteTraversal routeTraversal)
        {
            RouteTraversal = routeTraversal;
        }

        private float CalculateBaseWeight()
        {
            return 1.0f / RouteTraversal.Start.Routes.Count;
        }

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, hero, Sprite, RouteTraversal);
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("Travel Icon");
        }

        public override float GetPriority(Hero hero)
        {
            float staminaCost = (RouteTraversal.Length / hero.Movement.MoveSpeed) * HeroStamina.MOVEMENT_STAMINA_PER_HOUR;

            if (staminaCost > hero.Stamina.Current)
                return 0;

            return CalculateBaseWeight() * hero.Brain.BasePriorities.RandomTravel;
        }
    }
}
