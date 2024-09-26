using System;
using System.Collections.Generic;
using OlegHcp.Pool.Storages;

namespace OlegHcp.Pool
{
    /// <summary>
    /// Object pool implementation.
    /// </summary>
    public sealed class ObjectPool<T> where T : class
    {
        private IPoolStorage<T> _storage;
        private IObjectFactory<T> _factory;

        public int Count => _storage.Count;

        public ObjectPool(IPoolStorage<T> storage, IObjectFactory<T> factory)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _storage = storage;
            _factory = factory;
        }

        public ObjectPool(IPoolStorage<T> storage, Func<T> creator) : this(storage, new DefaultFactory(creator)) { }

        public ObjectPool(IObjectFactory<T> factory) : this(new QueueStorage<T>(16), factory) { }

        public ObjectPool(Func<T> creator) : this(new DefaultFactory(creator)) { }

        public void Fill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Release(_factory.Create());
            }
        }

        /// <summary>
        /// Returns an existing element or creates a new one if pool is empty.
        /// </summary>
        public T Get()
        {
            if (_storage.TryGet(out T value))
            {
                (value as IPoolable)?.Reinit();
                return value;
            }

            return _factory.Create();
        }

        /// <summary>
        /// Takes an object back to pool.
        /// </summary>
        public void Release(T obj)
        {
            (obj as IPoolable)?.CleanUp();
            if (!_storage.TryAdd(obj))
                (obj as IDisposable)?.Dispose();
        }

        public void Release(IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                Release(item);
            }
        }

        /// <summary>
        /// Clears object pool.
        /// </summary>
        public void Clear(bool disposeIfPossible = false)
        {
            if (disposeIfPossible)
            {
                while (_storage.TryGet(out T value))
                {
                    (value as IDisposable)?.Dispose();
                }
                return;
            }

            _storage.Clear();
        }

        private class DefaultFactory : IObjectFactory<T>
        {
            private Func<T> _create;

            public DefaultFactory(Func<T> creator)
            {
                if (creator == null)
                    throw new ArgumentNullException(nameof(creator));

                _create = creator;
            }

            T IObjectFactory<T>.Create()
            {
                return _create.Invoke();
            }
        }
    }
}
