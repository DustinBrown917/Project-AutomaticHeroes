using UnityEngine;
using NaughtyAttributes;
using SegoLib.DataProviders;

namespace SegoLib.Random
{
    /// <summary>
    /// Returns a value between 0 and 1 (inclusive) based on a curve.
    /// </summary>
    [CreateAssetMenu(fileName = "New Distribution Curve", menuName = "Random/Distribution Curve")]
    public class DistributionCurve : ScriptableObject, IDataProvider<float>, IIdentifiable<string>, IProbabilityMassProvider<float>
    {


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class ObjectLibrary : ObjectLibrary_string<DistributionCurve>
        {
            protected override void ApplyNewId(DistributionCurve obj, string id)
            {
                obj.name = id;
            }
        }

        private readonly struct WeightedValue
        {
            public readonly float cumulativeWeight;
            public readonly float individualWeight;
            public readonly float value;

            public WeightedValue(float cumulativeWeight, float individualWeight, float value)
            {
                this.cumulativeWeight = cumulativeWeight;
                this.individualWeight = individualWeight;
                this.value = value;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public static ObjectLibrary Library { get; } = new ObjectLibrary();

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        public string Id => name;

        [CurveRange(0, 0, 1, 1)]
        [SerializeField] private AnimationCurve m_curve;
        [SerializeField] private int m_precision = 50;
        private WeightedValue[] m_preCalcTable;
        private float m_highestWeight = 0;
        private float m_sumOfWeights = 0;

        public void Initialize()
        {
            m_preCalcTable = new WeightedValue[m_precision];
            NormalizeCurve();
        }

        private void NormalizeCurve()
        {
            float step = 1.0f / m_precision;

            m_sumOfWeights = 0;

            for (int i = 0; i < m_preCalcTable.Length; i++) {
                float value = step * i;
                float weight = Mathf.Clamp01(m_curve.Evaluate(value));
                m_sumOfWeights += weight;
                m_preCalcTable[i] = new WeightedValue(m_sumOfWeights, weight, value);

                if (weight > m_highestWeight)
                    m_highestWeight = weight;
            }
        }

        public float GetValue()
        {
            float randomSum = UnityEngine.Random.value * m_sumOfWeights;
            int index = 0;
            while (randomSum > m_preCalcTable[index].cumulativeWeight) {
                index++;
            }

            float nextVal = 1;
            if (index != m_preCalcTable.Length - 1) {
                nextVal = m_preCalcTable[index + 1].value;
            }

            float previousWeight = 0;
            if (index != 0) {
                previousWeight = m_preCalcTable[index - 1].cumulativeWeight;
            }

            float t = Mathf.InverseLerp(previousWeight, m_preCalcTable[index].cumulativeWeight, randomSum);
            return Mathf.Lerp(m_preCalcTable[index].value, nextVal, t);
        }

        private float GetCumulativeWeight(float value)
        {
            int index = 0;
            while(value > m_preCalcTable[index].value) {
                index++;
            }

            float nextWeight = m_sumOfWeights;
            if (index != m_preCalcTable.Length - 1) {
                nextWeight = m_preCalcTable[index + 1].cumulativeWeight;
            }

            float previousValue = 0;
            if (index != 0) {
                previousValue = m_preCalcTable[index - 1].value;
            }

            float t = Mathf.InverseLerp(previousValue, m_preCalcTable[index].value, value);
            return Mathf.Lerp(m_preCalcTable[index].cumulativeWeight, nextWeight, t);
        }

        private float GetIndividualWeight(float value)
        {
            int index = 0;
            while (value > m_preCalcTable[index].value) {
                index++;
            }

            float nextWeight = m_sumOfWeights;
            if (index != m_preCalcTable.Length - 1) {
                nextWeight = m_preCalcTable[index + 1].individualWeight;
            }

            float previousValue = 0;
            if (index != 0) {
                previousValue = m_preCalcTable[index - 1].value;
            }

            float t = Mathf.InverseLerp(previousValue, m_preCalcTable[index].value, value);
            return Mathf.Lerp(m_preCalcTable[index].individualWeight, nextWeight, t);
        }

        public float GetProbabilityMass(float value)
        {
            float weight = GetIndividualWeight(value);

            return weight / m_highestWeight;
        }
    } 
}
