using Game.Flow;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    /// <summary>
    /// Posts a message to the action feed, then moves straight on. Lets graphs decide what's newsworthy, instead of code.
    /// <para>The message is formatted with the behaviour state's <c>Format</c>: <c>{0}</c> is the world and <c>{1}</c> is the
    /// hero, e.g. <c>"{1:cname} is having {1:his} wounds treated in {1:loc}."</c></para>
    /// </summary>
    [NodeWidth(300)]
    public class HeroBehaviourNode_PostToFeed : IHeroBehaviour.Node
    {
        [Input] public FlowPort entry;

        [SerializeField, TextArea] private string m_message;

        [Output] public FlowPort exit;

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit): return exit;
                default: return null;
            }
        }

        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State state = new();

            if (!string.IsNullOrEmpty(m_message))
                ActionFeed.Post(bb.Format(m_message), bb.WorldState.Time.ElapsedSeconds);

            ConcludeViaPort(state, exit);
            return state;
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds) { }
    }
}
