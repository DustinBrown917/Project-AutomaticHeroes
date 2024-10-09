using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SegoLib.Logging
{
    /// <summary>
    /// Handles runtime logging outside of the UnityEditor.
    /// </summary>
    public static class DLog
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        /// <summary>
        /// The data class for a single log entry.
        /// </summary>
        public class LogEntry
        {
            public string LogString { get; }
            public string StackTrace { get; }
            public LogType Type { get; }

            public LogEntry(string logString, string stackTrace, LogType type)
            {
                LogString = logString;
                StackTrace = stackTrace;
                Type = type;
            }

            public override string ToString()
            {
                return Type.ToString() + '\n' + LogString + '\n' + StackTrace;
            }
        }

        /// <summary>
        /// Comparer for LogEntrys.
        /// </summary>
        internal class LogEntryComparer : IEqualityComparer<LogEntry>
        {
            public int GetHashCode(LogEntry logEntry)
            {
                return 0;
            }

            public bool Equals(LogEntry x, LogEntry y)
            {
                return x != null
                       && y != null
                       && x.LogString.Equals(y.LogString)
                       && x.StackTrace.Equals(y.StackTrace)
                       && x.Type.ToString().Equals(y.Type.ToString());
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Enums ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public enum Volume : byte
        {
            Off,
            Quiet,
            Loud
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Enums ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        const int INITIAL_CHARACTER_CAPACITY = 15000;




        private static StringBuilder m_stringBuilder = new StringBuilder(INITIAL_CHARACTER_CAPACITY);

        /// <summary>
        /// When adding a new log, this is the key it will be given.
        /// </summary>
        private static uint m_nextKeyToAdd = 0;

        /// <summary>
        /// The log with this key will be removed should m_logEntries exceed the maximum number of logs.
        /// </summary>
        private static uint m_nextKeyToRemove = 0;


        /// <summary>
        /// The maximum number of logs to keep in memory.
        /// </summary>
        public static int LogCacheSize { get; set; } = 300;

        /// <summary>
        /// The cached logs.
        /// </summary>
        private static Dictionary<uint, LogEntry> m_logCache = new Dictionary<uint, LogEntry>(LogCacheSize);

        /// <summary>
        /// Public accessor for cached logs.
        /// </summary>
        public static IReadOnlyDictionary<uint, LogEntry> LogCache => m_logCache;



        /// <summary>
        /// If the bool at (int)LogType index is true, then that LogType is muted.
        /// </summary>                                            Error        Assert        Warning       Log           Exception
        private static Volume[] m_logTypeVolumes = new Volume[] { Volume.Loud, Volume.Quiet, Volume.Quiet, Volume.Quiet, Volume.Loud };



        /// <summary>
        /// Is logging enabled.
        /// </summary>
        private static bool m_enabled = false;

        /// <summary>
        /// Is logging enabled.
        /// </summary>
        public static bool Enabled
        {
            get => m_enabled;
            set {
                if (m_enabled == value) return;

                m_enabled = value;

                if (m_enabled) {
                    Application.logMessageReceivedThreaded += HandleLogMessageReceived;
                } else {
                    Application.logMessageReceivedThreaded -= HandleLogMessageReceived;
                }

                onEnabledChanged?.Invoke(!m_enabled, m_enabled);
            }
        }



        /// <summary>
        /// Fires whenever a log with a loud LogType is received.
        /// [key, logEntry]
        /// </summary>
        public static event Action<uint, LogEntry> onLoudLogReceived;

        /// <summary>
        /// Fires whenever the enabled state changes.
        /// [oldValue, newValue]
        /// </summary>
        public static event Action<bool, bool> onEnabledChanged;

        /// <summary>
        /// Fires when the volume changes for a LogType.
        /// [theLogTypeThatWasChanged, oldValue, newValue]
        /// </summary>
        public static event Action<LogType, Volume, Volume> onVolumeChanged;



        /// <summary>
        /// Set the volume for <paramref name="logType"/> logs.
        /// </summary>
        /// <param name="logType">The LogType to set the volume for.</param>
        /// <param name="isMuted">The volume to set the log to.</param>
        public static void SetLogTypeVolume(LogType logType, Volume newVolume)
        {
            Volume oldVolume = m_logTypeVolumes[(int)logType];

            if (newVolume == oldVolume) return;

            m_logTypeVolumes[(int)logType] = newVolume;

            onVolumeChanged?.Invoke(logType, oldVolume, newVolume);
        }



        /// <summary>
        /// Get the volume of <paramref name="logType"/> logs.
        /// </summary>
        /// <param name="logType"></param>
        /// <returns>The current volume of <paramref name="logType"/> logs.</returns>
        public static Volume GetLogTypeVolume(LogType logType)
        {
            return m_logTypeVolumes[(int)logType];
        }



        /// <summary>
        /// Copies all logs received to the clipboard.
        /// </summary>
        public static void CopyLogsToClipboard()
        {
            CopyLogEntryListToClipboard(m_logCache.Values);
        }



        /// <summary>
        /// Copies a specific log entry to the clipboard.
        /// </summary>
        /// <param name="logKey"></param>
        public static void CopyLogToClipboard(uint logKey)
        {
            if(m_logCache.TryGetValue(logKey, out LogEntry log)) {
                GUIUtility.systemCopyBuffer = log.ToString();
            }
        }



        /// <summary>
        /// Copies a specific log entry to the clipboard.
        /// </summary>
        /// <param name="logEntry"></param>
        public static void CopyLogToClipboard(LogEntry logEntry)
        {
            GUIUtility.systemCopyBuffer = logEntry.ToString();
        }



        /// <summary>
        /// Copies all logs received to the clipboard, less any duplicate logs.
        /// </summary>
        public static void CopyDistinctLogsToClipBoard()
        {
            IEnumerable<LogEntry> _logEntriesWithoutDuplicates = m_logCache.Values.Distinct(new LogEntryComparer());

            CopyLogEntryListToClipboard(_logEntriesWithoutDuplicates);
        }



        /// <summary>
        /// Copies a list of LogEntrys to the clipboard.
        /// </summary>
        /// <param name="logEntries"></param>
        private static void CopyLogEntryListToClipboard(IEnumerable<LogEntry> logEntries)
        {
            m_stringBuilder.Clear();

            foreach (LogEntry logEntry in logEntries) {
                m_stringBuilder.AppendLine(logEntry.ToString());
            }

            GUIUtility.systemCopyBuffer = m_stringBuilder.ToString();
        }



        /// <summary>
        /// Handles logs received by Unity's debug system.
        /// </summary>
        /// <param name="logString"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private static void HandleLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            Volume volume = GetLogTypeVolume(type);

            if (volume == Volume.Off) return;

            LogEntry newLogEntry = new LogEntry(logString, stackTrace, type);
            m_logCache.Add(m_nextKeyToAdd++, newLogEntry);

            if (m_logCache.Count > LogCacheSize) {
                m_logCache.Remove(m_nextKeyToRemove);
            }

            if (volume == Volume.Loud) onLoudLogReceived?.Invoke(m_nextKeyToAdd - 1, newLogEntry);
        }

    }
}

