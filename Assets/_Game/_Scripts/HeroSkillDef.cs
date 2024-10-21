using SegoLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Hero Skill Definition", menuName = "Game/HeroSkillDef")]
    public class HeroSkillDef : ScriptableObject, IDisplaySprite, IDisplayName
    {
        private const string RESOURCE_PATH = "HeroSkillDef Libraries/All HeroSkillDefs";
        public static HeroSkillDefLibrary All { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeLibrary()
        {
            All = Resources.Load<HeroSkillDefLibrary>(RESOURCE_PATH);

            if(All == null) {
                Debug.LogError($"Failed to load HeroSkillDefLibrary at Resources/{RESOURCE_PATH}.");
                return;
            }

            for(int i = 0; i < All.Count; i++) {
                All[i].RuntimeIndex = i;
            }
        }


        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Sprite DisplaySprite { get; private set; }

        [field: System.NonSerialized] public int RuntimeIndex { get; private set; }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(DisplayName))
                DisplayName = name;
        }
    }
}
