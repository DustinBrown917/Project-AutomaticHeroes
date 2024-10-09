using UnityEngine;

namespace SegoLib.Random
{
    public static class ZTable
    {
        /// <summary>
        /// The minimum standard deviation.
        /// </summary>
        private static float SDMin => m_standardDeviations[0];
        /// <summary>
        /// The maximum standard deviation.
        /// </summary>
        private static float SDMax => m_standardDeviations[^1];

        /// <summary>
        /// The minimum area under curve.
        /// </summary>
        private static float AUCMin => m_areaUnderCurve[0];
        /// <summary>
        /// The maximum area under curve.
        /// </summary>
        private static float AUCMax => m_areaUnderCurve[^1];

        private static float[] m_standardDeviations = new float[] {
            -4f,
            -3.9f,
            -3.8f,
            -3.7f,
            -3.6f,
            -3.5f,
            -3.4f,
            -3.3f,
            -3.2f,
            -3.1f,
            -3f,
            -2.9f,
            -2.8f,
            -2.7f,
            -2.6f,
            -2.5f,
            -2.4f,
            -2.3f,
            -2.2f,
            -2.1f,
            -2f,
            -1.9f,
            -1.8f,
            -1.7f,
            -1.6f,
            -1.5f,
            -1.4f,
            -1.3f,
            -1.2f,
            -1.1f,
            -1f,
            -0.9f,
            -0.8f,
            -0.7f,
            -0.6f,
            -0.5f,
            -0.4f,
            -0.3f,
            -0.2f,
            -0.1f,
            0f,
            0.1f,
            0.2f,
            0.3f,
            0.4f,
            0.5f,
            0.6f,
            0.7f,
            0.8f,
            0.9f,
            1f,
            1.1f,
            1.2f,
            1.3f,
            1.4f,
            1.5f,
            1.6f,
            1.7f,
            1.8f,
            1.9f,
            2f,
            2.1f,
            2.2f,
            2.3f,
            2.4f,
            2.5f,
            2.6f,
            2.7f,
            2.8f,
            2.9f,
            3f,
            3.1f,
            3.2f,
            3.3f,
            3.4f,
            3.5f,
            3.6f,
            3.7f,
            3.8f,
            3.9f,
            4f,
        };
        private static float[] m_areaUnderCurve = new float[] {
            0.0f,
            4.810095E-05f,
            7.236004E-05f,
            0.0001078248f,
            0.0001591444f,
            0.0002326667f,
            0.0003369749f,
            0.0004834831f,
            0.0006872118f,
            0.0009676814f,
            0.001349956f,
            0.001865864f,
            0.002555192f,
            0.003467023f,
            0.004661202f,
            0.006209642f,
            0.008197486f,
            0.01072404f,
            0.01390335f,
            0.01786429f,
            0.02274996f,
            0.02871639f,
            0.03593013f,
            0.04456526f,
            0.05479908f,
            0.066807f,
            0.08075646f,
            0.09680027f,
            0.1150694f,
            0.1356657f,
            0.1586548f,
            0.1840597f,
            0.2118549f,
            0.2419631f,
            0.2742526f,
            0.308537f,
            0.3445777f,
            0.382088f,
            0.4207397f,
            0.4601715f,
            0.5f,
            0.5398272f,
            0.5792591f,
            0.6179107f,
            0.6554211f,
            0.6914619f,
            0.7257464f,
            0.7580359f,
            0.7881442f,
            0.8159395f,
            0.8413444f,
            0.8643335f,
            0.88493f,
            0.9031992f,
            0.9192431f,
            0.9331926f,
            0.9452006f,
            0.9554344f,
            0.9640696f,
            0.9712834f,
            0.9772499f,
            0.9821356f,
            0.9860966f,
            0.9892759f,
            0.9918025f,
            0.9937903f,
            0.9953388f,
            0.9965329f,
            0.9974448f,
            0.9981341f,
            0.99865f,
            0.9990323f,
            0.9993128f,
            0.9995165f,
            0.999663f,
            0.9997673f,
            0.9998409f,
            0.9998922f,
            0.9999276f,
            0.9999518f,
            1.0f,
        };


        public static float GetAreaUnderCurve(float standardDeviations)
        {
            if (standardDeviations <= SDMin)
                return AUCMin;

            if (standardDeviations >= SDMax)
                return AUCMax;

            int index = System.Array.BinarySearch(m_standardDeviations, standardDeviations);

            // Exact match found.
            if (index >= 0) {
                return m_areaUnderCurve[index];
            }
            
            index = ~index;
            float t = Mathf.InverseLerp(m_standardDeviations[index - 1], m_standardDeviations[index], standardDeviations);

            return Mathf.Lerp(m_areaUnderCurve[index-1], m_areaUnderCurve[index], t);
        }

        public static float GetStandardDeviationsFromMean(float areaUnderCurve)
        {
            if (areaUnderCurve <= AUCMin)
                return SDMin;

            if (areaUnderCurve >= AUCMax)
                return SDMax;

            int index = System.Array.BinarySearch(m_areaUnderCurve, areaUnderCurve);

            // Exact match found.
            if (index >= 0) {
                return m_standardDeviations[index];
            }

            index = ~index;
            float t = Mathf.InverseLerp(m_areaUnderCurve[index - 1], m_areaUnderCurve[index], areaUnderCurve);

            return Mathf.Lerp(m_standardDeviations[index - 1], m_standardDeviations[index], t);
        }
    }
}
