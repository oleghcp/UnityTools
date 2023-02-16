using UnityEngine;

namespace UnityUtility.SingleScripts
{
    /// <summary>
    /// Represents implementation of ScriptableObject singleton with lazy initialization.
    /// </summary>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _instance;

        /// <summary>
        /// Static instance of ScriptableSingleton`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (_instance == null)
                {
                    if (!CreateInstanceAttribute.TryUse(out _instance))
                        _instance = CreateInstance<T>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Returns true if the instance is not null.
        /// </summary>
        public static bool Exists => _instance != null;
    }
}
