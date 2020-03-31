using System;
using UnityEngine;

namespace UnityUtility.Scripts
{
    /// <summary>
    /// Represents implementation of ScriptableObject singleton with lazy initialization.
    /// </summary>
    public abstract class SOSingleton<T> : ScriptableObject where T : SOSingleton<T>
    {
        private static T s_inst;

        /// <summary>
        /// Static instance of SOSingleton`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (s_inst == null)
                {
                    Attribute a = Attribute.GetCustomAttribute(typeof(T), typeof(CreateInstanceAttribute), true);
                    s_inst = a != null ? (a as CreateInstanceAttribute).Create() as T : CreateInstance<T>();
                }
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
