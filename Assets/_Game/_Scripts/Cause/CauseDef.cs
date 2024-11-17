using System;
using UnityEngine;

namespace Game.Causes
{
    [CreateAssetMenu(menuName = "Game/Causes/CauseDef")]
    public class CauseDef : ScriptableObject
    {
        private const string RESOURCE_PATH = "Cause Libraries/All CauseDefs";
        public static CauseDefLibrary All { get; private set; }
        public static CauseDef Causeless => All[0];

        public static bool IsInitialized { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeLibrary()
        {
            if (IsInitialized)
                return;

            All = Resources.Load<CauseDefLibrary>(RESOURCE_PATH);

            if (All == null) {
                Debug.LogError($"Failed to load HeroSkillDefLibrary at Resources/{RESOURCE_PATH}.");
                return;
            }

            for (int i = 0; i < All.Count; i++) {
                All[i].RuntimeIndex = i;
            }

            Debug.Log($"Causes Loaded. Count: {All.Count}");
        }

        [field: SerializeField] public Color Color { get; private set; }

        [field: NonSerialized] public int RuntimeIndex { get; private set; }
    }
}
