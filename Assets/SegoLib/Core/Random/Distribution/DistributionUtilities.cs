using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Distributions
{
    internal static class DistributionUtilities
    {
        /// <summary>
        /// Generate a normally distributed value from <paramref name="stdDev"/>.
        /// </summary>
        /// <param name="stdDev">Ideally between -4 and 4.</param>
        /// <returns>Normal cumulative density at <paramref name="stdDev"/> standard deviations from mean.</returns>
        static float GenerateNormalCumulativeDensity(float stdDev)
        {
            float p = 0.3275911f;
            float a1 = 0.254829592f;
            float a2 = -0.284496736f;
            float a3 = 1.421413741f;
            float a4 = -1.453152027f;
            float a5 = 1.061405429f;

            int sign;
            if (stdDev < 0.0)
                sign = -1;
            else
                sign = 1;

            float x = Mathf.Abs(stdDev) / Mathf.Sqrt(2.0f);
            float t = 1.0f / (1.0f + p * x);
            float erf = 1.0f - (((((a5 * t + a4) * t) + a3)
              * t + a2) * t + a1) * t * Mathf.Exp(-x * x);
            return 0.5f * (1.0f + sign * erf);
        }

        /// <summary>
        /// Find the index of the element within <paramref name="data"/> that has the nearest <paramref name="target"/> cumulativeDensity without being less than <paramref name="target"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FindBestIndexOfCumulativeDensity(IReadOnlyList<DistributionData> data, float target)
        {
            int low = 0;
            int high = data.Count - 1;

            while (low <= high) {
                int mid = (low + high) / 2;

                if (data[mid].cumulativeDensity == target) {
                    // Exact match found
                    return mid;
                } else if (data[mid].cumulativeDensity < target) {
                    low = mid + 1;
                } else {
                    high = mid - 1;
                }
            }

            // No exact match found, return the index of the closest element not less than the target value
            return Mathf.Clamp(low, 0, data.Count - 1);
        }

        /// <summary>
        /// Find the index of the element within <paramref name="data"/> that has the nearest <paramref name="target"/> value without being less than <paramref name="target"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FindBestIndexOfValue(IReadOnlyList<DistributionData> data, float target)
        {
            int low = 0;
            int high = data.Count - 1;

            while (low <= high) {
                int mid = (low + high) / 2;

                if (data[mid].value == target) {
                    // Exact match found
                    return mid;
                } else if (data[mid].value < target) {
                    low = mid + 1;
                } else {
                    high = mid - 1;
                }
            }

            // No exact match found, return the index of the closest element not less than the target value
            return Mathf.Clamp(low, 0, data.Count - 1);
        }
    }
}
