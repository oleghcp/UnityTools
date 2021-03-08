using System;
using UnityUtilityTools;
using UnityEngine;

namespace UnityUtility.SingleScripts
{
    /// <summary>
    /// Represents implementation of MonoBehaviour singleton. It has no dynamic creation of an instance.
    /// It should be saved in scene or should be created manually in runtime.
    /// </summary>
    public abstract class SingleBehaviour<T> : MonoBehaviour, IDisposable where T : SingleBehaviour<T>
    {
        private static T _inst;
        private static bool _locked;

        /// <summary>
        /// Static instance of SingleScript`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (_inst == null)
                {
                    if (_locked)
                        throw new InvalidOperationException($"The instance of {typeof(T).Name} is being configured. Avoid recursive calls.");

                    if ((_inst = FindObjectOfType<T>()) == null)
                        throw new ObjectNotFoundException($"There is no any instance of {typeof(T).Name}.");

                    _inst.Construct();
                }

                return _inst;
            }
        }

        /// <summary>
        /// Returns true if the instance is not null.
        /// </summary>
        public static bool Exists
        {
            get { return _inst != null; }
        }

        // Unity Callbacks //

        private void Awake()
        {
            if (_inst == null)
            {
                _locked = true;
                Construct();
                _inst = this as T;
                _locked = false;
            }
        }

        private void OnDestroy()
        {
            if (_inst != null)
                DisposeInternal();
        }

        // -- //

        public void Dispose()
        {
            if (gameObject.IsPrefab())
                throw Errors.DisposingNonEditable();

            hideFlags = HideFlags.None;
            gameObject.Destroy();
            DisposeInternal();
        }

        // -- //

        private void DisposeInternal()
        {
            _inst = null;
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
