using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WorldTime : MonoBehaviour
    {
        public const float ONE_REAL_SECOND_IS_ONE_WORLD_SECOND = 1;
        public const float ONE_REAL_SECOND_IS_ONE_WORLD_MINUTE = 60;
        public const float ONE_REAL_SECOND_IS_ONE_WORLD_HOUR = 3_600;
        public const float ONE_REAL_SECOND_IS_ONE_WORLD_DAY = 86_400;

        public const float HOURS_TO_SECONDS = 0.0002778f;
        public const float MINUTES_TO_SECONDS = 0.0166667f;

        public interface IListener
        {
            public void OnWorldTimeUpdate(float deltaSeconds);
        }

        public static float HoursToSeconds(float hours)
        {
            return hours * HOURS_TO_SECONDS;
        }




        public float timeScale = ONE_REAL_SECOND_IS_ONE_WORLD_SECOND;

        public double ElapsedSeconds => m_elapsedSeconds;
        private double m_elapsedSeconds = 0;

        private readonly HashSet<IListener> m_listeners = new();

        private readonly HashSet<IListener> m_listenersToRemove = new();
        private readonly HashSet<IListener> m_listenersToAdd = new();

        private void Update()
        {
            ExecuteRemovalQueue();
            ExecuteAdditionQueue();

            float deltaGameTime = Time.deltaTime * timeScale;

            if (deltaGameTime < 0)
                deltaGameTime = 0;

            m_elapsedSeconds += deltaGameTime;

            UpdateListeners(deltaGameTime);
        }

        private void OnDestroy()
        {
            m_listeners.Clear();
            m_listenersToRemove.Clear();
            m_listenersToAdd.Clear();
        }

        private void UpdateListeners(float deltaTime)
        {
            foreach (IListener listener in m_listeners)
                listener.OnWorldTimeUpdate(deltaTime);
        }

        private void ExecuteRemovalQueue()
        {
            foreach(IListener listener in m_listenersToRemove)
                m_listeners.Remove(listener);

            m_listenersToRemove.Clear();
        }

        private void ExecuteAdditionQueue()
        {
            foreach(IListener listener in m_listenersToAdd)
                m_listeners.Add(listener);

            m_listenersToAdd.Clear();
        }

        public void AddListener(IListener listener)
        {
            m_listenersToRemove.Remove(listener);
            if(!m_listeners.Contains(listener))
                m_listenersToAdd.Add(listener);
        }

        public void RemoveListener(IListener listener)
        {
            m_listenersToAdd.Remove(listener);

            if(m_listeners.Contains(listener))
                m_listenersToRemove.Add(listener);
        }
    }
}
