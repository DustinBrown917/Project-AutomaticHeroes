using System;
using UnityEngine;

namespace Game
{
    public static class ActionFeed
    {
        public class Message : IComparable<Message>
        {
            public readonly float time;
            public readonly string message;

            public Message(string message)
            {
                this.time = Time.unscaledDeltaTime;
                this.message = message;
            }

            public int CompareTo(Message other) => time.CompareTo(other.time);
        }

        public static event Action<Message> messagePosted;

        public static void Post(string message)
        {
            messagePosted?.Invoke(new Message(message));
        }
    } 
}
