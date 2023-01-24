using System;
using System.Collections.Generic;
using UnityUtility.Pool.Storages;

namespace UnityUtility.Pool
{
    /// <summary>
    /// Object pool implementation.
    /// </summary>
    public sealed class ObjectPool<T> where T : class, IPoolable
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

        public ObjectPool(IPoolStorage<T> storage, IObjectFactory<T> factory, int preCount) : this(storage, factory)
        {
            for (int i = 0; i < preCount; i++)
            {
                Release(factory.Create());
            }
        }

        public ObjectPool(IPoolStorage<T> storage, Func<T> creator) : this(storage, new DefaultFactory(creator)) { }

        public ObjectPool(IObjectFactory<T> factory) : this(new QueueStorage<T>(16), factory) { }

        public ObjectPool(Func<T> creator) : this(new DefaultFactory(creator)) { }

        public ObjectPool(IPoolStorage<T> storage, Func<T> creator, int preCount) : this(storage, new DefaultFactory(creator), preCount) { }

        public ObjectPool(IObjectFactory<T> factory, int preCount) : this(new QueueStorage<T>(preCount * 2), factory, preCount) { }

        public ObjectPool(Func<T> creator, int preCount) : this(new DefaultFactory(creator), preCount) { }

        /// <summary>
        /// Returns an existing element or creates a new one if pool is empty.
        /// </summary>
        public T Get()
        {
            if (_storage.TryGet(out T value))
            {
                value.Reinit();
                return value;
            }

            return _factory.Create();
        }

        /// <summary>
        /// Takes an object back to pool.
        /// </summary>
        public void Release(T obj)
        {
            obj.CleanUp();
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
        public void Clear(bool dispose = false)
        {
            if (dispose)
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
