using System;
using Tools;
using UnityEngine;

namespace UnityUtility.Scripts
{
    /// <summary>
    /// Represents implementation of MonoBehaviour singleton with lazy initialization.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour, IDisposable where T : MonoSingleton<T>
    {
        private static T s_inst;

        /// <summary>
        /// Static instance of MonoSingleton`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (s_inst == null)
                    s_inst = SingletonUtility.CreateInstance(ComponentUtility.CreateInstance<T>);

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

        public void Dispose()
        {
            if (gameObject.IsPrefab())
                throw Errors.DisposingNonEditable();

            hideFlags = HideFlags.None;
            gameObject.Destroy();
            f_dispose();
        }

        private void OnDestroy()
        {
            if (s_inst != null)
                f_dispose();
        }

        private void f_dispose()
        {
            s_inst = null;
            CleanUp();
        }

        /// <summary>
        /// Use it instead of OnDestroy.
        /// </summary>
        protected abstract void CleanUp();
    }
}
