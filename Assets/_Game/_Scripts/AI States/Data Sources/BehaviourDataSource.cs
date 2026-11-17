using UnityEngine;

namespace Game.AI.General
{
    /// <summary>
    /// Reads one value from a running hero behaviour, so graph nodes can be given data without knowing where it comes from.
    /// <para>
    /// Each derived type reads a different thing. Build an asset for each value you need, then reference it from any node.
    /// </para>
    /// <para>
    /// Sources are shared assets, like graph nodes, so they must be stateless: everything they need comes from <c>bb</c>.
    /// </para>
    /// </summary>
    public abstract class BehaviourDataSource : ScriptableObject
    {
        /// <summary>
        /// The value for this behaviour state, or null if it doesn't have one.
        /// </summary>
        public abstract object GetData(IHeroBehaviour.IState bb);
    }
}
