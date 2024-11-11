using SegoLib;

namespace Game
{
    public static class InjectedDataIdentities
    {
        public static Identity RestExhaustionRecoveryRate { get; } = Identity<Identity>.GetNew(nameof(RestExhaustionRecoveryRate));
        public static Identity PreviousCombatSummary { get; } = Identity<Identity>.GetNew(nameof(PreviousCombatSummary));
    } 
}
