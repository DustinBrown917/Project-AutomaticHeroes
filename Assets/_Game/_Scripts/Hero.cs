using SegoLib;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HeroLevel))]
    [RequireComponent(typeof(HeroBrain))]
    [RequireComponent(typeof(HeroMovement))]
    public class Hero : MonoBehaviour
    {
        public HeroColor Color { get; private set; }
        public HeroLevel Level { get; private set; }
        public HeroMovement Movement { get; private set; }
        public HeroBrain Brain { get; private set; }

        public bool Initialized { get; private set; }

        private void Awake()
        {
            Movement = GetComponent<HeroMovement>();
            Level = GetComponent<HeroLevel>();
            Brain = GetComponent<HeroBrain>();
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

            Initialized = true;
        }
    }
}
