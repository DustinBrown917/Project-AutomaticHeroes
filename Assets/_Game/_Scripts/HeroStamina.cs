using SegoLib;
using UnityEngine;

namespace Game
{
    public class HeroStamina : HeroComponent
    {
        public const float DEFAULT_STAMINA_PER_HOUR = 6.25f;
        public const float DEFAULT_STAMINA_PER_SECOND = DEFAULT_STAMINA_PER_HOUR / 3600f;

        public const float MOVEMENT_STAMINA_PER_HOUR = 8.4f;
        public const float MOVEMENT_STAMINA_PER_SECOND = MOVEMENT_STAMINA_PER_HOUR / 3600f;
        public const float TRAINING_STAMINA_PER_HOUR = 16f;
        public const float TRAINING_STAMINA_PER_SECOND = TRAINING_STAMINA_PER_HOUR / 3600f;

        public const float CULL_STAMINA_PER_HOUR = MOVEMENT_STAMINA_PER_HOUR;
        public const float CULL_STAMINA_PER_SECOND = CULL_STAMINA_PER_HOUR / 3600f;

        public const float ENERGY_PERCENT_RECOVERED_PER_HOUR = 0.15f;
        public const float ENERGY_PERCENT_RECOVERED_PER_SECOND = ENERGY_PERCENT_RECOVERED_PER_HOUR / 3600;

        public float TotalStaminaPool
        {
            get => m_totalStaminaPool;
            set => SetTotalStaminaPool(value);
        }
        private float m_totalStaminaPool;
        public event ValueChangedWithSourceEvent<HeroStamina, float> totalStaminaPoolChanged;

        public float Energy
        {
            get => m_energy;
            set => SetCurrentEnergy(value);
        }
        private float m_energy;
        public event ValueChangedWithSourceEvent<HeroStamina, float> energyChanged;

        public float Exhaustion
        {
            get => m_exhaustion;
            set => SetExhaustion(value);
        }
        private float m_exhaustion;
        public event ValueChangedWithSourceEvent<HeroStamina, float> exhaustionChanged;


        public float EnergyPercent { get; private set; }

        public float ExhaustionPercent { get; private set; }



        public float MaxEnergy => TotalStaminaPool - Exhaustion;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized) return;

            TotalStaminaPool = 100;
            Energy = TotalStaminaPool;

            Initialized = true;
        }

        public void SetCurrentEnergy(float newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, MaxEnergy);

            if (m_energy == newValue)
                return;

            float oldValue = m_energy;
            m_energy = newValue;

            UpdateEnergyPercent();
            energyChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetTotalStaminaPool(float newValue)
        {
            if (newValue < 0) newValue = 0;

            if (m_totalStaminaPool == newValue)
                return;

            float oldValue = m_totalStaminaPool;
            m_totalStaminaPool = newValue;

            if (m_totalStaminaPool > Energy)
                Energy = m_totalStaminaPool;

            UpdateEnergyPercent();
            UpdateExuastionPercent();
            totalStaminaPoolChanged?.Invoke(this, oldValue, newValue);
        }

        public void SetExhaustion(float newValue)
        {
            newValue = Mathf.Clamp(newValue, 0, TotalStaminaPool);

            if (newValue == m_exhaustion)
                return;

            float oldValue = m_exhaustion;
            m_exhaustion = newValue;

            UpdateExuastionPercent();

            if(Energy > MaxEnergy)
                Energy = MaxEnergy;

            exhaustionChanged?.Invoke(this, oldValue, newValue);
        }

        public bool TrySpendEnergy(float amount)
        {
            if(amount > m_energy) return false;
            SpendEnergy(amount);
            return true;
        }

        public bool SpendEnergy(float amount)
        {
            bool depleted = amount > Energy;

            amount = Mathf.Min(amount, Energy);
            Energy -= amount;

            Exhaustion += amount * 0.1f;

            return !depleted;
        }

        public void RecoverEnergy(float deltaSeconds)
        {
            Energy += GetEnergyRecoveryPerSecond() * deltaSeconds;
        }

        public void RecoverExhaustion(float amount)
        {
            amount = Mathf.Min(amount, m_exhaustion);

            Exhaustion -= amount;

            Energy += amount;
        }

        private void UpdateEnergyPercent()
        {
            if (TotalStaminaPool == 0)
                EnergyPercent = 0;

            EnergyPercent = Energy / TotalStaminaPool;
        }

        private void UpdateExuastionPercent()
        {
            if (TotalStaminaPool == 0)
                ExhaustionPercent = 0;

            ExhaustionPercent = Exhaustion / TotalStaminaPool;
        }

        private float GetEnergyRecoveryPerSecond()
        {
            return ENERGY_PERCENT_RECOVERED_PER_SECOND * TotalStaminaPool;
        }
    } 
}
