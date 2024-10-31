using SegoLib.Random;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class RandomFlavourBank
    {
        [SerializeField] private string[] m_flavours = System.Array.Empty<string>();

        public string GetRandomFlavour(IRandomNumberGenerator rng)
        {
            return rng.GetRandom(m_flavours);
        }
    } 
}
