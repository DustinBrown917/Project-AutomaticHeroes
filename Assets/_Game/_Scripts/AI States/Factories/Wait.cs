using UnityEngine;

namespace Game
{
    public class Wait : StateFactory<Wait.State>
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            private float m_targetTime;
            private float m_elapsedTime = 0;

            public State(Wait stateFactory, Hero hero, float duration) : base(stateFactory, hero)
            {
                m_targetTime = duration;
            }


            public override void DoState(float deltaSeconds)
            {
                m_elapsedTime += deltaSeconds;

                Progress = m_elapsedTime / m_targetTime;

                if (m_elapsedTime >= m_targetTime) {
                    CompleteState();
                }
            }

            public override void EnterState() { }
            public override void ExitState() { }

            public override string GetActionString() => "Waiting...";
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        private float m_waitDuration = 4;

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, hero, m_waitDuration);
        }
    } 
}
