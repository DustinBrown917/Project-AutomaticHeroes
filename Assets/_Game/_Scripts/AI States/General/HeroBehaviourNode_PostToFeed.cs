using Game.Flow;
using System;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    /// <summary>
    /// Posts a message to the action feed, then moves straight on. Lets graphs decide what's newsworthy, instead of code.
    /// <para>
    /// The message is a format string filled from <see cref="m_formatData"/>: the source at index <c>n</c> fills <c>{n}</c>.
    /// For example, with a hero source then a last-combat-monster source: <c>"{0:cname} was downed by a {1}!"</c>
    /// </para>
    /// </summary>
    [NodeWidth(300)]
    public class HeroBehaviourNode_PostToFeed : IHeroBehaviour.Node
    {
        [Input] public FlowPort entry;

        [SerializeField, TextArea] private string m_message;

        [Tooltip("Values for the message. The source at index n fills {n}.")]
        [SerializeField] private BehaviourDataSource[] m_formatData;

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

            if (!string.IsNullOrEmpty(m_message) && TryFormatMessage(bb, out string message))
                ActionFeed.Post(message, bb.WorldState.Time.ElapsedSeconds);

            ConcludeViaPort(state, exit);
            return state;
        }

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds) { }

        private bool TryFormatMessage(IHeroBehaviour.IState bb, out string message)
        {
            int count = m_formatData != null ? m_formatData.Length : 0;
            object[] args = new object[count];

            for (int i = 0; i < count; i++) {
                args[i] = m_formatData[i] != null ? m_formatData[i].GetData(bb) : null;
            }

            try {
                message = string.Format(m_message, args);
                return true;
            } catch (FormatException e) {
                // A typo in a message shouldn't stop the hero's behaviour, so report it and carry on without posting.
                Debug.LogError($"Couldn't format feed message \"{m_message}\" in {graph.name} with {count} data source(s): {e.Message}", this);
                message = null;
                return false;
            }
        }
    }
}
