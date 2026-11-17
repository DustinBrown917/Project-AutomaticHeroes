using UnityEngine;

namespace Game.AI.General
{
    /// <summary>
    /// The hero running the behaviour, or one of its properties.
    /// <para>
    /// With no property, this returns the <see cref="Hero"/> itself, which supports the hero's format codes
    /// (<c>{0:cname}</c>, <c>{0:his}</c>, <c>{0:loc}</c>…). With a property, it returns that value, e.g. health percent
    /// formatted as <c>{0:P0}</c>.
    /// </para>
    /// </summary>
    [CreateAssetMenu(fileName = "New Hero Data Source", menuName = "Game/AI/Data Sources/Hero")]
    public class BehaviourDataSource_Hero : BehaviourDataSource
    {
        [Tooltip("Optional. A HeroProperty to read from the hero. Leave empty to return the hero itself.")]
        [SerializeField] private HeroProperty m_property;

        public override object GetData(IHeroBehaviour.IState bb)
        {
            if (m_property != null)
                return m_property.GetBoxed(bb.Hero);

            return bb.Hero;
        }
    }
}
