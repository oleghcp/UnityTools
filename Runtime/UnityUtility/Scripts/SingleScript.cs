using System;
using UnityEngine;

namespace UU.Scripts
{
    /// <summary>
    /// Represents implementation of MonoBehaviour singleton. It has no dynamic creation of an instance.
    /// It should be saved in scene or should be created manually in runtime.
    /// </summary>
    public abstract class SingleScript<T> : Script, IDisposable where T : SingleScript<T>
    {
        private static T s_inst;

        /// <summary>
        /// Static instance of SingleScript`1.
        /// </summary>
        public static T I
        {
            get
            {
                if (s_inst == null)
                {
                    s_inst = FindObjectOfType<T>();
                    if (s_inst == null)
                        throw new ObjectNotFoundException($"There is no any instance of {typeof(T).Name}.");
                    s_inst.Construct();
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

        // Unity Callbacks //

        private void Awake()
        {
            if (s_inst == null)
            {
                Construct();
                s_inst = this as T;
            }
        }

        private void OnDestroy()
        {
            if (s_inst != null)
                f_dispose();
        }

        // -- //

        public void Dispose()
        {
            if (gameObject.IsPrefab())
                throw new InvalidOperationException("Non-editable objects cannot be disposed. Probably it is a prefab reference.");

            hideFlags = HideFlags.None;
            gameObject.Destroy();
            f_dispose();
        }

        // -- //

        private void f_dispose()
        {
            s_inst = null;
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
