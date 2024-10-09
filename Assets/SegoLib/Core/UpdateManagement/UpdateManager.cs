using SegoLib.PlayerLoops;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

namespace SegoLib
{
    public static class UpdateManager
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        /// <summary>
        /// Information pertaining to a location within the UpdateManager.
        /// </summary>
        private readonly struct UpdatableLocation
        {
            public readonly UpdateIntervalHandler handler;
            public readonly int bucket;

            public UpdatableLocation(UpdateIntervalHandler handler, int bucket)
            {
                this.handler = handler;
                this.bucket = bucket;
            }
        }

        /// <summary>
        /// A collection of IUpdatables that will be updated together.
        /// </summary>
        private class UpdateBucket
        {
            /// <summary>
            /// The last time this bucket was updated.
            /// </summary>
            private float m_previousFrameTime = 0;
            /// <summary>
            /// The IUpdatables in this bucket.
            /// </summary>
            private readonly HashSet<IUpdatable> m_updatables = new HashSet<IUpdatable>();
            /// <summary>
            /// The number if IUpdatables in this bucket.
            /// </summary>
            public int Size => m_updatables.Count;

            public UpdateBucket()
            {
                m_previousFrameTime = Time.time;
            }

            public void Update(float currentFrameTime)
            {
                float deltaTime = currentFrameTime - m_previousFrameTime;
                m_previousFrameTime = currentFrameTime;

                foreach(IUpdatable updatable in m_updatables) {
                    updatable.Update(deltaTime);
                }
            }

            public void Register(IUpdatable updatable)
            {
                m_updatables.Add(updatable);
            }

            public bool Remove(IUpdatable updatable)
            {
                return m_updatables.Remove(updatable);
            }
        }

        /// <summary>
        /// Keeps a series of buckets and ensures they are updated every interval updates. Will attempt to spread newly registered updatables evenly between all managed buckets. <br/>
        /// Receives updates at maximum framerate and spreads them out across all buckets based on interval. Consider the following example with interval 3: <br/>
        /// Update 1 -> buckets[0].Update <br/>
        /// Update 2 -> buckets[1].Update <br/>
        /// Update 3 -> buckets[2].Update <br/>
        /// Update 4 -> buckets[0].Update <br/>
        /// Update 5 -> buckets[1].Update <br/>
        /// Update 6 -> buckets[2].Update <br/>
        /// </summary>
        private class UpdateIntervalHandler
        {
            public readonly int interval;
            private int m_currentBucket = 0;

            private UpdateBucket[] m_buckets;

            public UpdateIntervalHandler(int frequency)
            {
                if (frequency <= 0)
                    throw new ArgumentException("Must be greater than 0.", nameof(frequency));

                this.interval = frequency;

                m_buckets = new UpdateBucket[frequency];

                for (int i = 0; i < m_buckets.Length; i++) {
                    m_buckets[i] = new UpdateBucket();
                }
            }

            public void Update(float currentFrameTime)
            {
                m_buckets[m_currentBucket].Update(currentFrameTime);

                m_currentBucket++;
                if (m_currentBucket >= interval) {
                    m_currentBucket = 0;
                }
            }

            public int Register(IUpdatable updatable)
            {
                int indexOfSmallest = 0;
                UpdateBucket smallestBucket = m_buckets[indexOfSmallest];

                for (int i = 1; i < m_buckets.Length; i++) {
                    if (m_buckets[i].Size < smallestBucket.Size) {
                        smallestBucket = m_buckets[i];
                        indexOfSmallest = i;
                    }
                }

                smallestBucket.Register(updatable);
                return indexOfSmallest;
            }

            public bool Unregister(IUpdatable updatable, int bucket)
            {
                return m_buckets[bucket].Remove(updatable);
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        /// <summary>
        /// The player loop to be added to the player loop system.
        /// </summary>
        private static PlayerLoopSystem m_playerLoop = new PlayerLoopSystem() {
            type = typeof(UpdateManager),
            updateDelegate = UpdateManager.Update,
            subSystemList = null
        };


        /// <summary>
        /// All registered updatables paired with the frequency handler they are in.
        /// </summary>
        private static readonly Dictionary<IUpdatable, UpdatableLocation> m_updatables = new Dictionary<IUpdatable, UpdatableLocation>();
        /// <summary>
        /// Intervals paired with their respective interval handlers.
        /// </summary>
        private static readonly Dictionary<int, UpdateIntervalHandler> m_intervalHandlers = new Dictionary<int, UpdateIntervalHandler>();
        /// <summary>
        /// IUpdatables queued for addition paired with the interval they are to be updated at.
        /// </summary>
        private static readonly Dictionary<IUpdatable, int> m_updatablesQueuedForAddition = new Dictionary<IUpdatable, int>();
        /// <summary>
        /// IUpdatables queued for removal.
        /// </summary>
        private static readonly HashSet<IUpdatable> m_updatablesQueuedForRemoval = new HashSet<IUpdatable>();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopUtils.InsertSystem<UnityEngine.PlayerLoop.Update>(ref currentPlayerLoop, m_playerLoop, 0);
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);

            
#if UNITY_EDITOR
            // If in editor and reload domain is off, this becomes necessary.
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
#endif
        }

        /// <summary>
        /// Ensure any added updated loops are safely removed when exiting playmode.
        /// </summary>
        /// <param name="state"></param>
        private static void OnPlaymodeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopUtils.RemoveSystem<UnityEngine.PlayerLoop.Update>(ref currentPlayerLoop, in m_playerLoop);
                Clear();
            }
        }

        /// <summary>
        /// Update all IUpdatables as required.
        /// </summary>
        private static void Update()
        {
            HandleRemovalQueue();
            HandleRegistrationQueue();

            foreach (UpdateIntervalHandler frequencyHandler in m_intervalHandlers.Values) {
                frequencyHandler.Update(Time.time);
            }
        }

        /// <summary>
        /// Get or create an interval handler for interval <paramref name="interval"/>
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        private static UpdateIntervalHandler GetOrCreateIntervalHandler(int interval)
        {
            if (!m_intervalHandlers.TryGetValue(interval, out var handler)) {
                handler = new UpdateIntervalHandler(interval);
                m_intervalHandlers.Add(interval, handler);
            }

            return handler;
        }

        /// <summary>
        /// Clear the state of the UpdateManager.
        /// </summary>
        private static void Clear()
        {
            m_updatables.Clear();
            m_updatablesQueuedForAddition.Clear();
            m_updatablesQueuedForRemoval.Clear();
            m_intervalHandlers.Clear();
        }

        /// <summary>
        /// Register <paramref name="updatable"/> to be updated every <paramref name="interval"/> updates.
        /// </summary>
        /// <param name="updatable"></param>
        /// <param name="interval"></param>
        public static void Register(IUpdatable updatable, int interval = 1)
        {
            if (interval < 1) interval = 1;

            if (m_updatablesQueuedForRemoval.Contains(updatable)) {
                m_updatablesQueuedForRemoval.Remove(updatable);
            }

            m_updatablesQueuedForAddition[updatable] = interval;
        }

        /// <summary>
        /// Remove <paramref name="updatable"/> from update management.
        /// </summary>
        /// <param name="updatable"></param>
        public static void Unregister(IUpdatable updatable)
        {
            m_updatablesQueuedForAddition.Remove(updatable);
            m_updatablesQueuedForRemoval.Add(updatable);
        }

        /// <summary>
        /// Add all queued IUpdatables queued for addition to update management, then clear the queue.
        /// </summary>
        private static void HandleRegistrationQueue()
        {
            foreach (var kvp in m_updatablesQueuedForAddition) {
                RegisterInternal(kvp.Key, kvp.Value);
            }

            m_updatablesQueuedForAddition.Clear();
        }

        /// <summary>
        /// Remove all queued IUpdatables queued for removal from update management, then clear the queue.
        /// </summary>
        private static void HandleRemovalQueue()
        {
            foreach (IUpdatable updatable in m_updatablesQueuedForRemoval) {
                if (m_updatables.TryGetValue(updatable, out var location)) {
                    UnregisterInternal(updatable, location);
                }
            }

            m_updatablesQueuedForRemoval.Clear();
        }

        /// <summary>
        /// Register <paramref name="updatable"/> for updates every <paramref name="interval"/> updates. Performs necessary logic to ensure no duplicates exist.
        /// </summary>
        /// <param name="updatable"></param>
        /// <param name="interval"></param>
        private static void RegisterInternal(IUpdatable updatable, int interval)
        {
            if (m_updatables.TryGetValue(updatable, out UpdatableLocation location)) {
                if (location.handler.interval == interval) {
                    return;
                }

                location.handler.Unregister(updatable, location.bucket);
                m_updatables.Remove(updatable);
            }

            UpdateIntervalHandler handler = GetOrCreateIntervalHandler(interval);
            int bucket = handler.Register(updatable);

            m_updatables.Add(updatable, new UpdatableLocation(handler, bucket));
        }

        /// <summary>
        /// Remove <paramref name="updatable"/> from update management.
        /// </summary>
        /// <param name="updatable"></param>
        /// <param name="location"></param>
        private static void UnregisterInternal(IUpdatable updatable, UpdatableLocation location)
        {
            location.handler.Unregister(updatable, location.bucket);
            m_updatables.Remove(updatable);
        }
    }
}
