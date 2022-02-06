using System;
using System.Collections.Generic;

namespace UnityUtility.Collections
{
    public interface IPoolable
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

    /// <summary>
    /// Object pool implementation.
    /// </summary>
    public sealed class ObjectPool<T> where T : class, IPoolable
    {
        private Queue<T> _storage;
        private Func<T> _factory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">Reference to factory function.</param>
        public ObjectPool(Func<T> creator)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            _storage = new Queue<T>(16);
            _factory = creator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">Reference to factory function.</param>
        /// <param name="preCount">Count of precreated objects.</param>
        public ObjectPool(Func<T> creator, int preCount)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            _storage = new Queue<T>(preCount);
            _factory = creator;

            for (int i = 0; i < preCount; i++)
            {
                Release(_factory());
            }
        }

        /// <summary>
        /// Changes reference to creating function.
        /// </summary>        
        public void ChangeCreator(Func<T> creator)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            _factory = creator;
        }

        /// <summary>
        /// Returns an existing element or creates a new one if pool is empty.
        /// </summary>
        public T Get()
        {
            if (_storage.TryDequeue(out T value))
            {
                value.Reinit();
                return value;
            }

            return _factory();
        }

        /// <summary>
        /// Takes an object back to pool.
        /// </summary>
        public void Release(T obj)
        {
            obj.CleanUp();
            _storage.Enqueue(obj);
        }

        public void Release(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Release(item);
            }
        }

        /// <summary>
        /// Clears object pool.
        /// </summary>
        public void Clear()
        {
            _storage.Clear();
        }

        /// <summary>
        /// Clears object pool with disposing of each element.
        /// </summary>
        public void Clear(Action<T> disposer)
        {
            while (_storage.Count > 0)
            {
                disposer(_storage.Dequeue());
            }
        }
    }
}
