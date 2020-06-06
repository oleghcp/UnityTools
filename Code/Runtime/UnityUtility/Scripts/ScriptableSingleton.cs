using Tools;
using UnityEngine;

namespace UnityUtility.Scripts
{
    /// <summary>
    /// Represents implementation of ScriptableObject singleton with lazy initialization.
    /// </summary>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T s_inst;

        /// <summary>
        /// Static instance of ScriptableSingleton`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (s_inst == null)
                    s_inst = SingletonUtility.CreateInstance(CreateInstance<T>);

                return s_inst;
            }
        }

        /// <summary>
        /// Returns true if the instance is not null.
        /// </summary>
        public static bool Exists
        {
            get { return s_inst != null; }
        }
    }
}
