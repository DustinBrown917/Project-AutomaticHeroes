using SegoLib.Random;

namespace Game
{
    public class TravelToRandomNeighbour : StateFactory<TravelToRandomNeighbour.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            public State(TravelToRandomNeighbour stateFactory, Hero hero) : base(stateFactory, hero)
            {
                Hero.Movement.CurrentRoute = GetRandomRouteToNeighbour(Hero.Movement.Location, WorldState.Instance.Rng);
                Hero.Movement.routeCompleted += Movement_RouteCompleted;
            }

            private void Movement_RouteCompleted(HeroMovement heroMovement, IRouteTraversal route)
            {
                Hero.Movement.routeCompleted -= Movement_RouteCompleted;
                CompleteState();
            }

            public override void EnterState()
            {
                Hero.Movement.BeginMovement();
            }

            public override void DoState(float deltaSeconds)
            {
                Hero.Movement.ProgressBySeconds(deltaSeconds);
                Progress = Hero.Movement.RouteProgress;
            }

            public override void ExitState()
            {
                Hero.Movement.routeCompleted -= Movement_RouteCompleted;
            }

            public override string GetActionString() => $"Travelling to {Hero.Movement.CurrentRoute.End.DisplayName}.";

            private IRouteTraversal GetRandomRouteToNeighbour(ILocation location, IRandomNumberGenerator rng)
            {
                IRoute route = rng.GetRandom(location.Routes);

                IRouteTraversal routeTraversal;
                if (route.A == location) {
                    routeTraversal = new RouteTraversalAToB(route);
                } else {
                    routeTraversal = new RouteTraversalBToA(route);
                }

                return routeTraversal;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, hero);
        }
    }


}
