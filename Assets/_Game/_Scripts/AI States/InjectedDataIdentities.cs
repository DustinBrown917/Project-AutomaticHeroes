using SegoLib;

namespace Game
{
    public static class InjectedDataIdentities
    {
        public static Identity RestExhaustionRecoveryRate { get; } = Identity<Identity>.Get(nameof(RestExhaustionRecoveryRate));
        public static Identity PreviousCombatSummary { get; } = Identity<Identity>.Get(nameof(PreviousCombatSummary));
        public static Identity MonsterProviderPool { get; } = Identity<Identity>.Get(nameof(MonsterProviderPool));
        public static Identity TreatmentRate { get; } = Identity<Identity>.Get(nameof(TreatmentRate));
    } 
}
