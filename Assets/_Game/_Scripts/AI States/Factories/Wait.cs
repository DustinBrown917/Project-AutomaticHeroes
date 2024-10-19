using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Wait : StateFactory<Wait.State>
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : HeroState
        {
            private float m_targetTime;
            private float m_elapsedTime = 0;

            public State(Wait stateFactory, Sprite sprite, Hero hero, float duration) : base(stateFactory, sprite, hero)
            {
                m_targetTime = duration;
                ActionString = "Waiting...";
            }


            public override void DoState(float deltaSeconds)
            {
                m_elapsedTime += deltaSeconds;

                Progress = m_elapsedTime / m_targetTime;

                if (m_elapsedTime >= m_targetTime) {
                    SucceedState();
                }
            }

            public override void EnterState() { }
            public override void ExitState() { }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        [field: SerializeField] public float WaitDuration { get; private set; } = 4;

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, WaitDuration);
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("Wait Icon");
        }

        public override float GetPriority(Hero hero)
        {
            return 0f;
        }
    } 
}
