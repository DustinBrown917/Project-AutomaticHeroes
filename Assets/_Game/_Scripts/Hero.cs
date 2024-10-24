using SegoLib;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HeroLevel))]
    [RequireComponent(typeof(HeroBrain))]
    [RequireComponent(typeof(HeroMovement))]
    [RequireComponent(typeof(HeroGraphic))]
    [RequireComponent(typeof(HeroStamina))]
    [RequireComponent(typeof(HeroHealth))]
    [RequireComponent(typeof(HeroSkills))]
    [RequireComponent(typeof(HeroEquipment))]
    public class Hero : MonoBehaviour, WorldTime.IListener, IIdentifiable<int>
    {
        public int Id { get; private set; }

        public HeroCore Core { get; private set; }
        public HeroGraphic Graphic { get; private set; }
        public HeroLevel Level { get; private set; }
        public HeroMovement Movement { get; private set; }
        public HeroBrain Brain { get; private set; }
        public HeroHealth Health { get; private set; }
        public HeroStamina Stamina { get; private set; }
        public HeroSkills Skills { get; private set; }
        public HeroEquipment Equipment { get; private set; }

        public bool Initialized { get; private set; }


        private void Awake()
        {
            Movement = GetComponent<HeroMovement>();
            Level = GetComponent<HeroLevel>();
            Brain = GetComponent<HeroBrain>();
            Graphic = GetComponent<HeroGraphic>();
            Health = GetComponent<HeroHealth>();
            Stamina = GetComponent<HeroStamina>();
            Skills = GetComponent<HeroSkills>();
            Equipment = GetComponent<HeroEquipment>();
        }

        private void Start()
        {
            if(TryGetComponent(out UI.VM_Hero vm)) {
                vm.SetModel(this);
            }
        }

        private void OnDestroy()
        {
            if (WorldState.Instance != null && WorldState.Instance.Time != null) {
                WorldState.Instance.Time.RemoveListener(this);
            }

            Movement = null;
            Brain = null;
            Level = null;
            Graphic = null;
            Health = null;
            Stamina = null;
            Skills = null;
            Equipment = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">Where the hero starts. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Initialize(int id, HeroCore core, ILocation location)
        {
            if (Initialized)
                return;

            if(location == null) {
                throw new System.ArgumentNullException(nameof(location));
            }

            Id = id;

            Core = core;

            Skills.Initialize();
            Level.Initialize();
            Movement.Initialize(location);
            Graphic.Initialize();
            Health.Initialize();
            Stamina.Initialize();
            Equipment.Initialize();
            Brain.Initialize();

            WorldState.Instance.Time.AddListener(this);

            Initialized = true;
        }

        public void OnWorldTimeUpdate(float deltaSeconds)
        {
            Health.Recover(deltaSeconds);
            Stamina.RecoverEnergy(deltaSeconds);
        }
    }
}
