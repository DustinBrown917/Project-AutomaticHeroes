using System.Collections.Generic;

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
    }
}
