namespace SegoLib.Distributions
{
    public readonly struct DistributionData
    {
        public static implicit operator DistributionData((float value, float probMass, float cumDensity) data) => new DistributionData(data.value, data.probMass, data.cumDensity);
        public static implicit operator (float value, float probMass, float cumDensity)(DistributionData data) => (data.value, data.probabilityMass, data.cumulativeDensity);

        public readonly float value;
        public readonly float probabilityMass;
        public readonly float cumulativeDensity;

        public DistributionData(float value, float probabilityMass, float cumulativeDensity)
        {
            this.value = value;
            this.probabilityMass = probabilityMass;
            this.cumulativeDensity = cumulativeDensity;
        }
    }
}
