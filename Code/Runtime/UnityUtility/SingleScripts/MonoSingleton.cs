using System;
using UnityUtilityTools;
using UnityEngine;

namespace UnityUtility.SingleScripts
{
    /// <summary>
    /// Represents implementation of MonoBehaviour singleton with lazy initialization.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour, IDisposable where T : MonoSingleton<T>
    {
        private static T _inst;

        /// <summary>
        /// Static instance of MonoSingleton`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (_inst == null)
                    _inst = SingletonUtility.CreateInstance(ComponentUtility.CreateInstance<T>);

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

        public void Dispose()
        {
            if (gameObject.IsPrefab())
                throw Errors.DisposingNonEditable();

            hideFlags = HideFlags.None;
            gameObject.Destroy();
            DisposeInternal();
        }

        private void OnDestroy()
        {
            if (_inst != null)
                DisposeInternal();
        }

        private void DisposeInternal()
        {
            _inst = null;
            CleanUp();
        }

        /// <summary>
        /// Use it instead of OnDestroy.
        /// </summary>
        protected abstract void CleanUp();
    }
}
