using SegoLib;
using SegoLib.UI.MVVM;

namespace Game.UI
{
    public class VM_HeroStamina : VM_Base<HeroStamina>
    {

        public float TotalStaminaPool
        {
            get => m_totalStaminaPool;
            private set => PropertySetter.Set(ref m_totalStaminaPool, value, m_totalStaminaPool == value, OnPropertyChanged);
        }
        private float m_totalStaminaPool;

        public float Energy
        {
            get => m_energy;
            private set => PropertySetter.Set(ref m_energy, value, m_energy == value, OnPropertyChanged);
        }
        private float m_energy;

        public float Exhaustion
        {
            get => m_exhaustion;
            private set => PropertySetter.Set(ref m_exhaustion, value, m_exhaustion == value, OnPropertyChanged);
        }
        private float m_exhaustion;

        public float EnergyPercent
        {
            get => m_energyPercent;
            private set => PropertySetter.Set(ref m_energyPercent, value, m_energyPercent == value, OnPropertyChanged);
        }
        private float m_energyPercent;

        public float ExhaustionPercent
        {
            get => m_exhaustionPercent;
            private set => PropertySetter.Set(ref m_exhaustionPercent, value, m_exhaustionPercent == value, OnPropertyChanged);
        }
        private float m_exhaustionPercent;

        protected override bool IsEqual(HeroStamina a, HeroStamina b) => a == b;

        protected override void SetValuesToDefault()
        {
            Energy = 0;
            TotalStaminaPool = 0;
            EnergyPercent = 0;
            Exhaustion = 0;
        }

        protected override void UpdateValues()
        {
            Energy = Model.Energy;
            TotalStaminaPool = Model.TotalStaminaPool;
            UpdateEnergyPercent();
            Exhaustion = Model.Exhaustion;
            UpdateExhaustionPercent();
        }

        protected override void OnModelChangedToNotNull()
        {
            Model.energyChanged += Model_EnergyChanged;
            Model.totalStaminaPoolChanged += Model_TotalStaminaPoolChanged;
            Model.exhaustionChanged += Model_ExhaustionChanged;
        }


        protected override void OnModelChangingFromNotNull()
        {
            Model.energyChanged -= Model_EnergyChanged;
            Model.totalStaminaPoolChanged -= Model_TotalStaminaPoolChanged;
            Model.exhaustionChanged -= Model_ExhaustionChanged;
        }

        private void Model_TotalStaminaPoolChanged(HeroStamina source, float oldValue, float newValue)
        {
            TotalStaminaPool = newValue;
            UpdateEnergyPercent();
            UpdateExhaustionPercent();
        }

        private void Model_EnergyChanged(HeroStamina source, float oldValue, float newValue)
        {
            Energy = newValue;
            UpdateEnergyPercent();
        }

        private void Model_ExhaustionChanged(HeroStamina source, float oldValue, float newValue)
        {
            Exhaustion = newValue;
            UpdateExhaustionPercent();
        }

        private void UpdateEnergyPercent()
        {
            EnergyPercent = Model.EnergyPercent;
        }

        private void UpdateExhaustionPercent()
        {
            ExhaustionPercent = Model.ExhaustionPercent;
        }
    }
}
