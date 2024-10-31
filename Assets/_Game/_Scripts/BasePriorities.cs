using UnityEngine;

namespace Game
{
    public class BasePriorities
    {
        public float StaminaRecovery
        {
            get => m_staminaRecovery;
            set => SetFloat01(ref m_staminaRecovery, value);
        }
        private float m_staminaRecovery;

        public float RandomTravel
        {
            get => m_randomTravel;
            set => SetFloat01(ref m_randomTravel, value);
        }
        private float m_randomTravel;

        public float WoundTreatment
        {
            get => m_woundTreatment;
            set => SetFloat01(ref m_woundTreatment, value);
        }
        private float m_woundTreatment;

        public float Training
        {
            get => m_training;
            set => SetFloat01(ref m_training, value);
        }
        private float m_training;

        public float Combat
        {
            get => m_combat;
            set => SetFloat01(ref m_combat, value);
        }
        private float m_combat;

        public BasePriorities(float staminaRecovery, float randomTravel, float woundTreatment, float training, float combat)
        {
            StaminaRecovery = staminaRecovery;
            RandomTravel = randomTravel;
            WoundTreatment = woundTreatment;
            Training = training;
            Combat = combat;
        }

        private void SetFloat01(ref float property, float newValue)
        {
            property = Mathf.Clamp01(newValue);
        }
    }
}
