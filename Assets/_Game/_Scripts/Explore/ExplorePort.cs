using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Exploration
{
    [System.Serializable]
    public class ExplorePort
    {
        public enum EResolutionRequirement : byte
        {
            Any,
            Failure,
            Success
        }

        [SerializeField] private string m_name;
        [field: SerializeField] public EResolutionRequirement ResolutionRequirement { get; private set; }
        [field: SerializeField] public HeroSkillDef SkillUsed { get; private set; }
        [field: SerializeField] public float Weight { get; private set; }
        [field: System.NonSerialized] public ExploreNode ConnectedNode { get; private set; }

        public void SetConnectedNode(ExploreNode node)
        {
            ConnectedNode = node;
        }

        public bool IsValidForResolution(in NodeResolution resolution)
        {
            if (ResolutionRequirement == EResolutionRequirement.Failure && resolution.wasSuccessful)
                return false;

            if (ResolutionRequirement == EResolutionRequirement.Success && !resolution.wasSuccessful)
                return false;

            if (SkillUsed != null && resolution.skillUsed != SkillUsed)
                return false;

            return true;
        }
    }
}
