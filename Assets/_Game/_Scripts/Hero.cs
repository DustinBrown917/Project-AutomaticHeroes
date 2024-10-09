using SegoLib;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HeroBrain))]
    [RequireComponent(typeof(HeroMovement))]
    public class Hero : MonoBehaviour
    {
        public HeroMovement Movement { get; private set; }
        public HeroBrain Brain { get; private set; }

        public bool Initialized { get; private set; }

        private void Awake()
        {
            Movement = GetComponent<HeroMovement>();
            Brain = GetComponent<HeroBrain>();
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
        public void Initialize(ILocation location)
        {
            if (Initialized)
                return;

            if(location == null) {
                throw new System.ArgumentNullException(nameof(location));
            }

            Movement.Initialize(location);
            Brain.Initialize();

            Initialized = true;
        }
    }
}
