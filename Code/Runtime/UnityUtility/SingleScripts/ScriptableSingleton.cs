using UnityEngine;
using UnityUtility.Engine;
using UnityUtility.Tools;

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
                    if (!CreateInstanceAttribute.TryUse<T>())
                        CreateInstance<T>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Returns true if the instance is not null.
        /// </summary>
        public static bool Exists => _instance != null;

        private void OnEnable()
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

        private void OnDisable()
        {
            _instance = null;
            Destruct();
        }

        public void Dispose()
        {
            this.Destroy();
            _instance = null;
        }

        /// <summary>
        /// Used it instead of Awake.
        /// </summary>
        protected abstract void Construct();

        /// <summary>
        /// Used it instead of OnDestroy.
        /// </summary>
        protected abstract void Destruct();
    }
}
