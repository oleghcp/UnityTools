using System;
using OlegHcp.Engine;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.SingleScripts
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
                    if (!CreateInstanceAttribute.TryUse<T>())
                        ComponentUtility.CreateInstance<T>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Returns true if the instance is not null.
        /// </summary>
        public static bool Exists => _instance != null;

        private void Awake()
        {
            if (_instance != null)
            {
                if (this != _instance)
                    DebugErrors.MultipleInstancesMessage<T>();

                return;
            }

            Construct();
            _instance = this as T;
        }

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
        /// Used it instead of Awake.
        /// </summary>
        protected abstract void Construct();

        /// <summary>
        /// Use it instead of OnDestroy.
        /// </summary>
        protected abstract void Destruct();
    }
}
