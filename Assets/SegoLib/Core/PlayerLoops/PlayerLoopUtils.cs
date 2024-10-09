using System.Collections.Generic;
using System.Text;
using UnityEngine.LowLevel;

namespace SegoLib.PlayerLoops
{
    public static class PlayerLoopUtils
    {
        /// <summary>
        /// Get all current player loops as a string.
        /// </summary>
        /// <param name="playerLoop"></param>
        /// <returns></returns>
        public static string GetPlayerLoopString(PlayerLoopSystem playerLoop)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Unity Player Loop");
            if (playerLoop.subSystemList != null) {
                foreach (PlayerLoopSystem subsystem in playerLoop.subSystemList) {
                    AppendPlayerLoopString(subsystem, sb, 0);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Recursively build the player loop string. Used by GetPlayerLoopString().
        /// </summary>
        /// <param name="system"></param>
        /// <param name="sb"></param>
        /// <param name="level"></param>
        private static void AppendPlayerLoopString(PlayerLoopSystem system, StringBuilder sb, int level)
        {
            sb.Append(' ', level * 2).AppendLine(system.type.Name);

            if (system.subSystemList == null || system.subSystemList.Length == 0)
                return;

            foreach (PlayerLoopSystem subSystem in system.subSystemList) {
                AppendPlayerLoopString(subSystem, sb, level + 1);
            }
        }

        /// <summary>
        /// Recursively searches <paramref name="system"/> for a PlayerLoopSystem with system.Type that is type <typeparamref name="T"/>. Once found, <paramref name="subSystem"/> is
        /// inserted at index <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="system"></param>
        /// <param name="subSystem"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static bool InsertSystem<T>(ref PlayerLoopSystem system, in PlayerLoopSystem subSystem, int index)
        {
            if (system.type != typeof(T))
                return HandleSubSystemLoopForInsertion<T>(ref system, subSystem, index);

            List<PlayerLoopSystem> playerLoopSystemList = new List<PlayerLoopSystem>();
            if (system.subSystemList != null)
                playerLoopSystemList.AddRange(system.subSystemList);

            playerLoopSystemList.Insert(index, subSystem);
            system.subSystemList = playerLoopSystemList.ToArray();
            return true;
        }

        /// <summary>
        /// Recursive handling for InsertSystem().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="system"></param>
        /// <param name="subSystem"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool HandleSubSystemLoopForInsertion<T>(ref PlayerLoopSystem system, in PlayerLoopSystem subSystem, int index)
        {
            if (system.subSystemList == null) {
                return false;
            }

            for (int i = 0; i < system.subSystemList.Length; i++) {
                if (!InsertSystem<T>(ref system.subSystemList[i], in subSystem, index))
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Recursively search <paramref name="system"/> for a PlayerLoopSystem that is of type <typeparamref name="T"/>. Once found, removes <paramref name="subSystem"/> from the found PlayerLoopSystem.subSystemList.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="system"></param>
        /// <param name="subSystem"></param>
        internal static void RemoveSystem<T>(ref PlayerLoopSystem system, in PlayerLoopSystem subSystem)
        {
            if (system.subSystemList == null)
                return;

            List<PlayerLoopSystem> playerLoopSystemList = new List<PlayerLoopSystem>(system.subSystemList);

            for (int i = 0; i < playerLoopSystemList.Count; i++) {
                if (playerLoopSystemList[i].type == subSystem.type && playerLoopSystemList[i].updateDelegate == subSystem.updateDelegate) {
                    playerLoopSystemList.RemoveAt(i);
                    system.subSystemList = playerLoopSystemList.ToArray();
                }
            }


            HandleSubSystemLoopForRemoval<T>(ref system, subSystem);
        }

        /// <summary>
        /// Recursive handling for RemoveSystem().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="system"></param>
        /// <param name="subSystem"></param>
        private static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem system, PlayerLoopSystem subSystem)
        {
            if (system.subSystemList == null)
                return;

            for (int i = 0; i < system.subSystemList.Length; i++) {
                RemoveSystem<T>(ref system.subSystemList[i], subSystem);
            }
        }
    } 
}
