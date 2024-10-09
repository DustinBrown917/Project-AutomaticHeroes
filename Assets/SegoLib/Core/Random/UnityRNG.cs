using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Random
{
    public class UnityRNG : IRandomNumberGenerator
    {
        private static UnityRNG m_activeRNG = null;

        private static void Activate(UnityRNG rng)
        {
            if(rng == m_activeRNG) {
                return;
            }

            if(m_activeRNG != null) {
                m_activeRNG.StoreState(UnityEngine.Random.state);
            }

            if(rng != null) {
                if(rng.m_storedState != null) {
                    UnityEngine.Random.state = rng.m_storedState.Value;
                } else {
                    UnityEngine.Random.InitState(rng.Seed);
                }
            }

            m_activeRNG = rng;
        }

        private static int m_seed = -1;

        public int Seed {
            get => m_seed;
            set {
                if (m_seed == value)
                    return;

                m_storedState = null;

                if(m_activeRNG == this)
                    UnityEngine.Random.InitState(m_seed);

                m_seed = value;
            }
        }

        private UnityEngine.Random.State? m_storedState;

        public UnityRNG() { }

        public UnityRNG(int seed)
        {
            Seed = seed;
        }

        /// <summary>
        /// When this RNG is no longer
        /// </summary>
        /// <param name="state"></param>
        private void StoreState(UnityEngine.Random.State state)
        {
            m_storedState = state;
        }

        public float Float01()
        {
            Activate(this);
            return UnityEngine.Random.value;
        }

        public float Range(float minInclusive, float maxInclusive)
        {
            Activate(this);
            return UnityEngine.Random.Range(minInclusive, maxInclusive);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            Activate(this);
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }

        public T GetRandom<T>(IReadOnlyList<T> list)
        {
            if (list == null)
                throw new System.ArgumentNullException(nameof(list));

            if (list.Count == 0)
                throw new System.ArgumentException(nameof(list), $"{nameof(list)} must contain at least one element.");

            Activate(this);
            return list[Range(0, list.Count)];
        }

        public float Range(FloatRange rangeInclusive)
        {
            return Range(rangeInclusive.Min, rangeInclusive.Max);
        }

        public int Range(IntRange rangeInclusive)
        {
            return Range(rangeInclusive.Min, rangeInclusive.Max + 1);
        }

        public bool Bool(float chanceOfTrue = 0.5f) => Float01() <= chanceOfTrue;
    }
}
