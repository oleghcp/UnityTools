using System;

namespace UnityUtility.Pool
{
    public interface IPoolable : IDisposable
    {
        /// <summary>
        /// Called when pool gives an existing object away.
        /// </summary>
        void Reinit();

        /// <summary>
        /// Called when object is returned to pool.
        /// </summary>
        void CleanUp();
    }
}
