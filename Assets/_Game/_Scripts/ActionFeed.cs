using System;
using UnityEngine;

namespace Game
{
    public static class ActionFeed
    {
        public class Message
        {
            public readonly double time;
            public readonly string message;

            public Message(string message, double time)
            {
                this.time = time;
                this.message = message;
            }
        }

        public static event Action<Message> messagePosted;

        public static void Post(string message, double time)
        {
            messagePosted?.Invoke(new Message(message, time));
        }
    } 
}
