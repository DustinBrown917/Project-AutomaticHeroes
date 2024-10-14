

using System.Diagnostics;
using UnityEngine;

namespace Game
{
    public class Training_General : StateFactory<Training_General.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : State_Base
        {
            private const float EXP_PER_SECOND = 8f / 3600;

            public float duration = 6;
            public float elapsed = 0;

            public int expReward;

            public State(Training_General stateFactory, Hero hero, int expReward) : base(stateFactory, hero)
            {
                this.expReward = expReward;
                duration = expReward / EXP_PER_SECOND;
            }

            public override void DoState(float deltaSeconds)
            {
                elapsed += deltaSeconds;

                Progress = elapsed / duration;

                Hero.Level.GainExperience(deltaSeconds * EXP_PER_SECOND);

                if(elapsed > duration) {
                    CompleteState();
                }
            }

            public override void EnterState() { }

            public override void ExitState() { }

            public override string GetActionString() => "Training.";
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        public int expReward = 10;

        public override Sprite Sprite { get; }

        public Training_General()
        {
            Sprite = SpriteLibrary.GetSprite("Training Icon");
        }

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, hero, expReward);
        }
    } 
}
