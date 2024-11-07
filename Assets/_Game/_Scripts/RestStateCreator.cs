using SegoLib;

namespace Game
{
    public class RestStateCreator : HeroBehaviourStateCreator
    {
        public float exhaustionRecoveredPerHour = 3;
        public string stateText = "Resting.";

        protected override (Identity key, object value)[] GetInjectedData(WorldState worldState, Hero hero)
        {
            return new (Identity key, object value)[] { (InjectedDataIdentities.RestExhaustionRecoveryRate, WorldTime.HoursToSeconds(exhaustionRecoveredPerHour)) };
        }

        protected override void OnStateCreated(WorldState worldState, Hero hero, IHeroBehaviour.IState createdState)
        {
            createdState.StateText = stateText;
        }

        public override float Utility(WorldState worldState, Hero hero)
        {
            return (hero.Stamina.Exhaustion + hero.Core.TimeOfDayPreference.GetPreferenceForCurrentTime()) * 0.5f;
        }
    } 
}
