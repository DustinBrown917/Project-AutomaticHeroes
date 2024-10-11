
namespace Game
{
    public class DoLocationAction : StateFactory<DoLocationAction.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            private float m_elapsedSeconds;
            private float m_secondsToComplete = 4;


            public State(DoLocationAction stateFactory, Hero hero) : base(stateFactory, hero) { }

            public override void EnterState() { }

            public override void DoState(float deltaSeconds)
            {
                m_elapsedSeconds += deltaSeconds;
                Progress = m_elapsedSeconds / m_secondsToComplete;

                if (m_elapsedSeconds >= m_secondsToComplete) {
                    CompleteState();
                }
            }

            public override void ExitState() { }

            public override string GetActionString() => "Doing a thing.";
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
