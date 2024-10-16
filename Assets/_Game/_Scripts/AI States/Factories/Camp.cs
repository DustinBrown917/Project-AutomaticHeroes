using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Camp : Rest
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public new class State : Rest.State
        {
            public State(Camp stateFactory, Sprite sprite, Hero hero, float staminaRecoveredPerHour) : base(stateFactory, sprite, hero, staminaRecoveredPerHour)
            {
            }

            public override string GetActionString() => "Resting at camp.";
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        public Camp() : base()
        {
            RestQuality = RestQuality.Terrible;
        }

        protected override Rest.State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, RestQuality.StaminaRecoveredPerHour());
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("fire");
        }

        public override float GetPriority(Hero hero)
        {
            return base.GetPriority(hero) * 0.5f; 
        }
    } 
}
