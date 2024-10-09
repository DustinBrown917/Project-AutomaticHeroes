using SegoLib.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WorldState : MonoBehaviour
    {
        public static WorldState Instance { get; private set; }

        public IRandomNumberGenerator Rng { get; private set; }

        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else {
                Rng = new UnityRNG((int)System.DateTime.Now.Ticks);
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    } 
}
