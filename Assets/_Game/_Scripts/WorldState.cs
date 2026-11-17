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

        public WorldTime Time { get; private set; }
        public WorldDate Date { get; private set; }

        [field: SerializeField] public LocationManager Locations { get; private set; }
        [field: SerializeField] public HeroFactory HeroFactory { get; private set; }

        private void Awake()
        {
            if (Instance != null) {
                Destroy(this);
                return;
            }

            Rng = new UnityRNG((int)System.DateTime.Now.Ticks);
            Time = GetComponentInChildren<WorldTime>();
            Date = GetComponentInChildren<WorldDate>();

            if (Locations != null)
                Locations.Initialize();
            else
                Debug.LogError($"{nameof(WorldState)} has no {nameof(LocationManager)} assigned.", this);

            if (HeroFactory != null)
                HeroFactory.Initialize(this);
            else
                Debug.LogError($"{nameof(WorldState)} has no {nameof(HeroFactory)} assigned.", this);

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
            Locations = null;
            HeroFactory = null;
        }
    } 
}
