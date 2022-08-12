using System;
using UnityEngine;
using UnityUtilityTools;

namespace UnityUtility.SingleScripts
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
        /// Static instance of SingleScript`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (_instance == null)
                {
                    T instance = FindObjectOfType<T>();

                    if (instance == null)
                        throw new ObjectNotFoundException($"There is no any instance of {typeof(T).Name}.");

                    if (instance._locked)
                        throw new InvalidOperationException($"The instance of {typeof(T).Name} is being configured. Avoid recursive calls.");

                    (_instance = instance).Construct();
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
            if (_instance == null)
            {
                _locked = true;
                Construct();
                _instance = this as T;
                _locked = false;
            }
        }

        private void OnDestroy()
        {
            if (_instance != null)
                DisposeInternal();
        }

        public void Dispose()
        {
            if (gameObject.IsAsset())
                throw Errors.DisposingNonEditable();

            hideFlags = HideFlags.None;
            gameObject.Destroy();
            DisposeInternal();
        }

        private void DisposeInternal()
        {
            _instance = null;
            CleanUp();
        }

        /// <summary>
        /// Used it instead of Awake.
        /// </summary>
        protected abstract void Construct();

        /// <summary>
        /// Used it instead of OnDestroy.
        /// </summary>
        protected abstract void CleanUp();
    }
}
