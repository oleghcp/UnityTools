using Debug = UnityEngine.Debug;

using System;
using System.Diagnostics;
using UnityEngine;

namespace UnityUtility
{
    /// <summary>
    /// UnityEngine.Debug wrapper which is created mostly to hide the logger property from substitution in C# text editor.
    /// </summary>
    public static class Msg
    {
        private const string CHECK_MSG = "Check passed";
        private const string ASSERT_MSG = "Assertion failed";

        /// <summary>
        /// Event for the custom console or terminal implementation.
        /// </summary>
        public static event Action<LogType, object> Log_Event;

        /// <summary>
		/// Logs message to the Unity Console.
		/// </summary>
        public static void Log(object msg)
        {
            Debug.Log(msg);
            Log_Event?.Invoke(LogType.Log, msg);
        }

        /// <summary>
        /// A variant of the Log function that logs a warning message to the console.
        /// </summary>
        /// <param name="msg"></param>
        public static void Warning(object msg)
        {
            Debug.LogWarning(msg);
            Log_Event?.Invoke(LogType.Warning, msg);
        }

        /// <summary>
        /// A variant of the Log function that logs an error message to the console.
        /// </summary>
        public static void Error(object msg)
        {
            Debug.LogWarning(msg);
            Log_Event?.Invoke(LogType.Error, msg);
        }

        /// <summary>
        /// Asserts a condition and logs an error message to the Unity console on failure.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
            if (!condition)
                Log_Event?.Invoke(LogType.Assert, ASSERT_MSG);
        }

        /// <summary>
        /// Asserts a condition and logs an error message to the Unity console on failure.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message)
        {
            Debug.Assert(condition, message);
            if (!condition)
                Log_Event?.Invoke(LogType.Assert, message);
        }

        /// <summary>
        /// Checks a condition and logs a message to the Unity console on success.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Check(bool condition)
        {
            if (condition)
            {
                Debug.Log(CHECK_MSG);
                Log_Event?.Invoke(LogType.Log, CHECK_MSG);
            }
        }

        /// <summary>
        /// Checks a condition and logs a message to the Unity console on success.
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Check(bool condition, object message)
        {
            if (condition)
            {
                Debug.Log(message);
                Log_Event?.Invoke(LogType.Log, message);
            }
        }
    }
}
