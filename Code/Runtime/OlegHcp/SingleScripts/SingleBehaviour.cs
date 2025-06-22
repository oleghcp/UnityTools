using System;
using OlegHcp.Engine;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.SingleScripts
{
    /// <summary>
    /// Represents implementation of MonoBehaviour singleton. It has no dynamic creation of an instance.
    /// It should be saved in scene or should be created manually in runtime.
    /// </summary>
    public abstract class SingleBehaviour<T> : MonoBehaviour, IDisposable where T : SingleBehaviour<T>
    {
        private static T _instance;

        private bool _locked;

        /// <summary>
        /// Static instance of SingleBehaviour`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_2023_1_OR_NEWER
                    T instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
#elif UNITY_2020_1_OR_NEWER
                    T instance = FindObjectOfType<T>(true);
#else
                    T instance = FindObjectOfType<T>();
#endif

                    if (instance == null)
                        throw new ObjectNotFoundException(typeof(T));

                    if (instance._locked)
                        throw new InvalidOperationException($"The instance of {typeof(T).Name} is being configured. Avoid recursive calls.");

                    instance.Initialize();
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

            Initialize();
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

        private void Initialize()
        {
            _locked = true;
            Construct();
            _instance = this as T;
            _locked = false;
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
