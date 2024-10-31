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

        public Wait DefaultWaitStateFactory { get; private set; } = new Wait();

        public WorldTime Time { get; private set; }
        public WorldDate Date { get; private set; }

        public HeroFormatProvider HeroFormatProvider { get; private set; } = new HeroFormatProvider();

        private void Awake()
        {
            if (Instance != null) {
                Destroy(this);
                return;
            }

            Rng = new UnityRNG((int)System.DateTime.Now.Ticks);
            Time = GetComponentInChildren<WorldTime>();
            Date = GetComponentInChildren<WorldDate>();

            if(TryGetComponent(out VM_WorldDate vm_worldDate)) {
                vm_worldDate.SetModel(Date);
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            Time = null;
            Date = null;
        }
    } 
}
