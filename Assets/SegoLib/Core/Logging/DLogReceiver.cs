using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.Logging
{
    /// <summary>
    /// A component that fires a UnityEvent whenever a loud log is received.
    /// </summary>
    public class DLogReceiver : MonoBehaviour
    {
        [Tooltip("Raised when a something is logged to a LogType with a Loud volume.")]
        public UnityEvent<uint, DLog.LogEntry> onLoudLogReceived;

        private void Awake()
        {
            DLog.onLoudLogReceived += OnLoudLogReceived;
        }

        private void OnLoudLogReceived(uint logId, DLog.LogEntry logEntry)
        {
            onLoudLogReceived?.Invoke(logId, logEntry);
        }

        private void OnDestroy()
        {
            DLog.onLoudLogReceived -= OnLoudLogReceived;
        }
    } 
}
