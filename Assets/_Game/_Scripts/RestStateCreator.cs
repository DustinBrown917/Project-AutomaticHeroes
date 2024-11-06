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

        protected override void OnStateCreated(IHeroBehaviour.IState createdState)
        {
            createdState.StateText = stateText;
        }
    } 
}
