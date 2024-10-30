
namespace Game.Exploration
{
    public readonly struct NodeResolution
    {
        public readonly HeroSkillDef skillUsed;
        public readonly bool wasSuccessful;

        public NodeResolution(HeroSkillDef skillUsed, bool wasSuccessful)
        {
            this.skillUsed = skillUsed;
            this.wasSuccessful = wasSuccessful;
        }
    }
}