﻿using System;
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
        private Stack<T> _stack;
        private Func<T> _createFunc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="creator">Reference to creating function.</param>
        public ObjectPool(Func<T> creator)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            _stack = new Stack<T>();
            _createFunc = creator;
        }

        /// <summary>
        /// Changes reference to creating function.
        /// </summary>        
        public void ChangeCreator(Func<T> creator)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            _createFunc = creator;
        }

        /// <summary>
        /// Creates objects and put them to pool.
        /// </summary>
        /// <param name="preCount">Number of objects.</param>
        public void Precreate(int preCount)
        {
            for (int i = 0; i < preCount; i++)
            {
                Release(_createFunc());
            }
        }

        /// <summary>
        /// Returns an existing element or creates a new one if pool is empty.
        /// </summary>
        public T Get()
        {
            if (_stack.Count == 0) { return _createFunc(); }

            T obj = _stack.Pop();
            obj.Reinit();
            return obj;
        }

        /// <summary>
        /// Takes an object back to pool.
        /// </summary>
        public void Release(T obj)
        {
            obj.CleanUp();
            _stack.Push(obj);
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
            _stack.Clear();
        }

        /// <summary>
        /// Clears object pool with disposing of each element.
        /// </summary>
        public void Clear(Action<T> disposer)
        {
            while (_stack.Count > 0)
            {
                disposer(_stack.Pop());
            }
        }
    }
}
