using System;
using System.Collections.Generic;

namespace SegoLib.Random
{
    public static class WeightedRandom
    {
        /// <summary>
        /// Get a random index via subtractive search. Good for smaller collection sizes.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="sumOfWeights"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static int Naive(IReadOnlyList<float> weights, float sumOfWeights, IRandomNumberGenerator rng)
        {
            float rand = rng.Float01() * sumOfWeights;

            for(int i = 0; i < weights.Count; i++) {
                rand -= weights[i];
                if(rand <= 0) {
                    return i;
                }
            }

            return weights.Count - 1;
        }

        /// <summary>
        /// Get a random index via subtractive search. Good for smaller collection sizes.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="sumOfWeights"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static int Naive(IReadOnlyList<int> weights, int sumOfWeights, IRandomNumberGenerator rng)
        {
            int rand = (int)(rng.Float01() * sumOfWeights);

            for (int i = 0; i < weights.Count; i++) {
                rand -= weights[i];
                if (rand <= 0) {
                    return i;
                }
            }

            return weights.Count - 1;
        }

        /// <summary>
        /// Get a random index via subtractive search. Good for smaller collection sizes.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="sumOfWeights"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static int Naive<T>(IReadOnlyList<T> items, float sumOfWeights, IRandomNumberGenerator rng, Func<T, float> getItemWeight)
        {
            float rand = rng.Float01() * sumOfWeights;

            for (int i = 0; i < items.Count; i++) {
                rand -= getItemWeight(items[i]);
                if (rand <= 0) {
                    return i;
                }
            }

            return items.Count - 1;
        }

        /// <summary>
        /// Get a random index via binary search. Good for larger collection sizes.
        /// </summary>
        /// <param name="cumulativeWeights">
        /// The cumulative weights of each index, where each element is the sum of explicit weights that came before it, plus its own explicit weight. <br/>
        /// ex. if <paramref name="cumulativeWeights"/> = [1, 2.5, 3, 5], then the explicit weights of each element looks like this: [1, 1.5, 0.5, 2].
        /// </param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static int Binary(List<float> cumulativeWeights, IRandomNumberGenerator rng)
        {
            float rand = rng.Float01() * cumulativeWeights[cumulativeWeights.Count - 1];

            int index = cumulativeWeights.BinarySearch(rand);

            if (index < 0)
                index = ~index;

            return index;
        }

        /// <summary>
        /// Get a random index via binary search. Good for larger collection sizes.
        /// </summary>
        /// <param name="cumulativeWeights">
        /// The cumulative weights of each index, where each element is the sum of explicit weights that came before it, plus its own explicit weight. <br/>
        /// ex. if <paramref name="cumulativeWeights"/> = [1, 2.5, 3, 5], then the explicit weights of each element looks like this: [1, 1.5, 0.5, 2].
        /// </param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static int Binary(List<int> cumulativeWeights, IRandomNumberGenerator rng)
        {
            int rand = (int)(rng.Float01() * cumulativeWeights[cumulativeWeights.Count - 1]);

            int index = cumulativeWeights.BinarySearch(rand);

            if (index < 0)
                index = ~index;

            return index;
        }

        /// <summary>
        /// Get a random index via binary search. Good for larger collection sizes.
        /// </summary>
        /// <param name="cumulativeWeights">
        /// The cumulative weights of each index, where each element is the sum of explicit weights that came before it, plus its own explicit weight. <br/>
        /// ex. if <paramref name="cumulativeWeights"/> = [1, 2.5, 3, 5], then the explicit weights of each element looks like this: [1, 1.5, 0.5, 2].
        /// </param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static int Binary(float[] cumulativeWeights, IRandomNumberGenerator rng)
        {
            float rand = rng.Float01() * cumulativeWeights[cumulativeWeights.Length - 1];

            int index = System.Array.BinarySearch(cumulativeWeights, rand);

            if (index < 0)
                index = ~index;

            return index;
        }

        /// <summary>
        /// Get a random index via binary search. Good for larger collection sizes.
        /// </summary>
        /// <param name="cumulativeWeights">
        /// The cumulative weights of each index, where each element is the sum of explicit weights that came before it, plus its own explicit weight. <br/>
        /// ex. if <paramref name="cumulativeWeights"/> = [1, 2.5, 3, 5], then the explicit weights of each element looks like this: [1, 1.5, 0.5, 2].
        /// </param>
        /// <param name="rng"></param>
        /// <returns></returns>
        public static int Binary(int[] cumulativeWeights, IRandomNumberGenerator rng)
        {
            int rand = (int)(rng.Float01() * cumulativeWeights[cumulativeWeights.Length - 1]);

            int index = System.Array.BinarySearch(cumulativeWeights, rand);

            if (index < 0)
                index = ~index;

            return index;
        }

    } 
}
