
using System.Collections.Generic;

namespace SegoLib.Random
{
    public interface IRandomNumberGenerator 
    {
        /// <summary>
        /// The seed of this random number generator.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// Returns a random float within [0.0..1.0] (range is inclusive)
        /// </summary>
        /// <returns></returns>
        public float Float01();

        /// <summary>
        /// Returns a random float within [<paramref name="minInclusive"/>..<paramref name="maxInclusive"/>] (range is inclusive).
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxInclusive"></param>
        /// <returns></returns>
        public float Range(float minInclusive, float maxInclusive);
        /// <summary>
        /// Returns a random float within [<paramref name="minInclusive"/>..<paramref name="maxExclusive"/>]. Range is not inclusive of <paramref name="maxExclusive"/>.
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxExclusive"></param>
        /// <returns></returns>
        public int Range(int minInclusive, int maxExclusive);
    }

    public static class IRandomNumberGeneratorExtensions
    {
        /// <summary>
        /// Generates a value between 0 and 1 inclusive. Returns true if the generated value is less than or equal to <paramref name="chanceOfTrue"/>. Otherwise, false.
        /// </summary>
        /// <returns></returns>
        public static bool Bool(this IRandomNumberGenerator rng, float chanceOfTrue = 0.5f)
        {
            return rng.Float01() <= chanceOfTrue;
        }

        /// <summary>
        /// Gets a random item from <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rng"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown if list is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the list contains fewer than 1 element.</exception>
        public static T GetRandom<T>(this IRandomNumberGenerator rng, IReadOnlyList<T> list)
        {
            if (list == null)
                throw new System.ArgumentNullException(nameof(list));

            if (list.Count == 0)
                throw new System.ArgumentException(nameof(list), $"{nameof(list)} must contain at least one element.");

            return list[rng.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a random float within [<paramref name="rangeInclusive"/>.Min..<paramref name="rangeInclusive"/>.Max] (range is inclusive).
        /// </summary>
        /// <param name="rangeInclusive"></param>
        /// <returns></returns>
        public static float Range(this IRandomNumberGenerator rng, FloatRange rangeInclusive)
        {
            return rng.Range(rangeInclusive.Min, rangeInclusive.Max);
        }

        /// <summary>
        /// Returns a random integer within [<paramref name="rangeInclusive"/>.Min..<paramref name="rangeInclusive"/>.Max] (range is inclusive).
        /// </summary>
        /// <param name="rangeInclusive"></param>
        /// <returns></returns>
        public static int Range(this IRandomNumberGenerator rng, IntRange rangeInclusive)
        {
            return rng.Range(rangeInclusive.Min, rangeInclusive.Max + 1);
        }
    }

}
