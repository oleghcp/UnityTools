using System;
using UnityEngine;
using UnityUtilityTools;

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
            if (_instance != null)
                DisposeInternal();
        }

        public void Dispose()
        {
            if (gameObject.IsAsset())
                throw Errors.DisposingNonEditable();

            gameObject.Destroy();
            DisposeInternal();
        }

        private void DisposeInternal()
        {
            _instance = null;
            Destruct();
        }

        /// <summary>
        /// Use it instead of OnDestroy.
        /// </summary>
        protected abstract void Destruct();
    }
}
