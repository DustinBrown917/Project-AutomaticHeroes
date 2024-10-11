

namespace Game
{
    public class Training_General : StateFactory<Training_General.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            private const float EXP_PER_SECOND = 1.5f;

            public float duration = 6;
            public float elapsed = 0;

            public int expReward;

            public State(Training_General stateFactory, Hero hero, int expReward) : base(stateFactory, hero)
            {
                this.expReward = expReward;
                duration = expReward * EXP_PER_SECOND;
            }

            public override void DoState(float deltaSeconds)
            {
                elapsed += deltaSeconds;

                Progress = elapsed / duration;

                if(elapsed > duration) {
                    CompleteState();
                }
            }

            public override void EnterState() { }

            public override void ExitState() { }

            public override string GetActionString() => "Training.";

            protected override void OnComplete()
            {
                Hero.Level.GainExperience(expReward);
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, hero, WorldState.Instance.Rng.Range(2, 10));
        }
    } 
}
