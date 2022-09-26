using System;
using UnityEngine;

namespace UnityUtility.SingleScripts
{
    /// <summary>
    /// Represents implementation of MonoBehaviour singleton with lazy initialization.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour, IDisposable where T : MonoSingleton<T>
    {
        private static T _instance;

        /// <summary>
        /// Static instance of MonoSingleton`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (_instance == null)
                {
                    if (!SingletonUtility.TryUseAttribute(out _instance))
                        _instance = ComponentUtility.CreateInstance<T>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Returns true if the instance is not null.
        /// </summary>
        public static bool Exists => _instance != null;

        private void OnDestroy()
        {
            _instance = null;
            Destruct();
        }

        public void Dispose()
        {
            gameObject.Destroy();
            _instance = null;
        }

        /// <summary>
        /// Use it instead of OnDestroy.
        /// </summary>
        protected abstract void Destruct();
    }
}
