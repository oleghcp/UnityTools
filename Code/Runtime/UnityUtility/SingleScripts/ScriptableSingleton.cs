using UnityEngine;

namespace UnityUtility.SingleScripts
{
    /// <summary>
    /// Represents implementation of ScriptableObject singleton with lazy initialization.
    /// </summary>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _inst;

        /// <summary>
        /// Static instance of ScriptableSingleton`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (_inst == null)
                    _inst = SingletonUtility.CreateInstance(CreateInstance<T>);

                return _inst;
            }
        }

        /// <summary>
        /// Returns true if the instance is not null.
        /// </summary>
        public static bool Exists => _inst != null;
    }
}
