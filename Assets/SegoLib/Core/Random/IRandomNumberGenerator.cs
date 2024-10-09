
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
        /// Returns a random float within [<paramref name="rangeInclusive"/>.Min..<paramref name="rangeInclusive"/>.Max] (range is inclusive).
        /// </summary>
        /// <param name="rangeInclusive"></param>
        /// <returns></returns>
        public float Range(FloatRange rangeInclusive);
        /// <summary>
        /// Returns a random float within [<paramref name="minInclusive"/>..<paramref name="maxExclusive"/>]. Range is not inclusive of <paramref name="maxExclusive"/>.
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxExclusive"></param>
        /// <returns></returns>
        public int Range(int minInclusive, int maxExclusive);
        /// <summary>
        /// Returns a random float within [<paramref name="rangeInclusive"/>.Min..<paramref name="rangeInclusive"/>.Max] (range is inclusive).
        /// </summary>
        /// <param name="rangeInclusive"></param>
        /// <returns></returns>
        public int Range(IntRange rangeInclusive);
        /// <summary>
        /// Gets a random item from <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public T GetRandom<T>(IReadOnlyList<T> list);
        /// <summary>
        /// Generates a value between 0 and 1 inclusive. Returns true if the generated value is less than or equal to <paramref name="chanceOfTrue"/>. Otherwise, false.
        /// </summary>
        /// <returns></returns>
        public bool Bool(float chanceOfTrue = 0.5f);
    } 
}
