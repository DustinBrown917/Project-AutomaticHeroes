using SegoLib;
using System;
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
    public partial class Hero : MonoBehaviour, WorldTime.IListener, IIdentifiable<int>
    {
        public int Id { get; private set; }

        /// <summary>
        /// The world this hero lives in. Assigned in <see cref="Initialize"/>, so components can rely on it from their own <c>OnInitialize</c> onwards.
        /// </summary>
        public WorldState WorldState { get; private set; }

        public HeroCore Core { get; private set; }
        public HeroGraphic Graphic { get; private set; }
        public HeroLevel Level { get; private set; }
        public HeroMovement Movement { get; private set; }
        public HeroBrain Brain { get; private set; }
        public HeroHealth Health { get; private set; }
        public HeroStamina Stamina { get; private set; }
        public HeroSkills Skills { get; private set; }
        public HeroEquipment Equipment { get; private set; }
        public HeroHeuristics Heuristics { get; private set; }

        public bool IsInitialized { get; private set; }

        public bool IsDestroyed { get; private set; }

        public event Action<Hero> Destroyed;

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
            Heuristics = GetComponent<HeroHeuristics>();
        }

        private void Start()
        {
            if(TryGetComponent(out UI.VM_Hero vm)) {
                vm.SetModel(this);
            }
        }

        private void OnDestroy()
        {
            IsDestroyed = true;
            Destroyed?.Invoke(this);
            Destroyed = null;

            if (WorldState != null && WorldState.Time != null) {
                WorldState.Time.RemoveListener(this);
            }

            WorldState = null;
            Movement = null;
            Brain = null;
            Level = null;
            Graphic = null;
            Health = null;
            Stamina = null;
            Skills = null;
            Equipment = null;
            Heuristics = null;
        }

        /// <summary>
        /// Set up the hero and all of its components, then start receiving world time updates. Does nothing if already initialized.
        /// </summary>
        /// <param name="id">Unique id for this hero.</param>
        /// <param name="core">The hero's identity. Cannot be null.</param>
        /// <param name="worldState">The world this hero lives in. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Initialize(int id, HeroCore core, WorldState worldState)
        {
            if (IsInitialized)
                return;

            if (core == null)
                throw new ArgumentNullException(nameof(core));

            if (worldState == null)
                throw new ArgumentNullException(nameof(worldState));

            Id = id;
            Core = core;
            // Assigned before any component initializes, since the brain picks its first behaviour during initialization.
            WorldState = worldState;

            Skills.Initialize();
            Level.Initialize();
            Movement.Initialize();
            Graphic.Initialize();
            Health.Initialize();
            Stamina.Initialize();
            Equipment.Initialize();
            Heuristics.Initialize();
            Brain.Initialize();

            WorldState.Time.AddListener(this);

            IsInitialized = true;
        }

        public void OnWorldTimeUpdate(float deltaSeconds)
        {
            Health.Recover(deltaSeconds);
            Stamina.OnWorldTimeUpdate(deltaSeconds);
        }
    }




    public partial class Hero : System.IFormattable
    {
        public string ToString(string format, System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) return Core.Name;
            
            switch (format) {
                // Pronouns
                case "he":
                case "him":
                case "his":
                case "hers":
                case "self":
                    return Core.Gender.Pronouns.ToString(format, formatProvider);
                // Names
                case "cname": return Core.ColoredName;
                case "end": return Movement.CurrentRoute != null ? Movement.CurrentRoute.End.DisplayName : "nowhere";
                case "start": return Movement.CurrentRoute != null ? Movement.CurrentRoute.Start.DisplayName : "nowhere";
                case "loc": return Movement.Location != null ? Movement.Location.DisplayName : "nowhere";
                default: return Core.Name;
            }
        }
    }

}
