using System;
using System.Collections.Generic;
using OlegHcp.Tools;

#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public class ServiceLocatorLazy
    {
        private Dictionary<Type, Data> _storage = new Dictionary<Type, Data>();

        public void Register<T>(IObjectFactory<T> factory, bool error = true) where T : class, IService
        {
            if (factory == null)
                throw ThrowErrors.NullParameter(nameof(factory));

            if (_storage.TryAdd(typeof(T), new Data(factory)))
                return;

            if (error)
                throw new InvalidOperationException($"Service {typeof(T)} already registered.");
        }

        public void Register<T>(Func<T> creator, bool error = true) where T : class, IService
        {
            if (creator == null)
                throw ThrowErrors.NullParameter(nameof(creator));

            Register(new DefaultFactory<T>(creator), error);
        }

        public T Get<T>(bool error = true) where T : class, IService
        {
            if (_storage.TryGetValue(typeof(T), out Data value))
                return (T)value.Service;

            if (error)
                throw ThrowErrors.ServiceNotRegistered(typeof(T));

            return null;
        }

        public void ClearRegistrations()
        {
            _storage.Clear();
        }

        public void ClearInstances()
        {
            foreach (Data item in _storage.Values)
            {
                item.Clear();
            }
        }

        private class Data
        {
            private IService _service;
            private IObjectFactory<IService> _factory;

            public IService Service => _service ?? (_service = _factory.Create());

            public Data(IObjectFactory<IService> factory)
            {
                _factory = factory;
            }

            public void Clear()
            {
                _service = null;
            }
        }

        private class DefaultFactory<T> : IObjectFactory<T> where T : class, IService
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
