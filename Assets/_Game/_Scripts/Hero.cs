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
    public class Hero : MonoBehaviour
    {
        public HeroColor Color { get; private set; }
        public HeroGraphic Graphic { get; private set; }
        public HeroLevel Level { get; private set; }
        public HeroMovement Movement { get; private set; }
        public HeroBrain Brain { get; private set; }
        public HeroHealth Health { get; private set; }
        public HeroStamina Stamina { get; private set; }

        public bool Initialized { get; private set; }

        private void Awake()
        {
            Movement = GetComponent<HeroMovement>();
            Level = GetComponent<HeroLevel>();
            Brain = GetComponent<HeroBrain>();
            Graphic = GetComponent<HeroGraphic>();
            Health = GetComponent<HeroHealth>();
            Stamina = GetComponent<HeroStamina>();
        }

        private void Start()
        {
            if(TryGetComponent(out UI.VM_Hero vm)) {
                vm.SetModel(this);
            }
        }

        private void OnDestroy()
        {
            Movement = null;
            Brain = null;
            Level = null;
            Graphic = null;
            Health = null;
            Stamina = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">Where the hero starts. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Initialize(HeroColor color, ILocation location)
        {
            if (Initialized)
                return;

            if(location == null) {
                throw new System.ArgumentNullException(nameof(location));
            }

            Color = color;

            Level.Initialize();
            Movement.Initialize(location);
            Brain.Initialize();
            Graphic.Initialize(color);
            Health.Initialize();
            Stamina.Initialize();

            Initialized = true;
        }
    }
}
