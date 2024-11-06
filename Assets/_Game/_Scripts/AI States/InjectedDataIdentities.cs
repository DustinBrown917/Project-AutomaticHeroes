using SegoLib;

namespace Game
{
    public static class InjectedDataIdentities
    {
        public static Identity RestExhaustionRecoveryRate { get; } = Identity<Identity>.GetNew(nameof(RestExhaustionRecoveryRate));
    } 
}
