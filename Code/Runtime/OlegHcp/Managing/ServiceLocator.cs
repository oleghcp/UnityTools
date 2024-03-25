using System;
using System.Collections.Generic;
using OlegHcp.Tools;
using System.Runtime.Serialization;

#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public interface IService { }

    public interface IInitialContext<out TService> where TService : class, IService
    {
        TService GetOrCreateInstance();
    }

    public class ServiceLocator
    {
        private protected Dictionary<Type, ServiceLocatorData> _storage = new Dictionary<Type, ServiceLocatorData>();

        public TService Get<TService>(bool error = true) where TService : class, IService
        {
            if (_storage.TryGetValue(typeof(TService), out ServiceLocatorData value))
                return (TService)value.Service;

            if (error)
                throw ThrowErrors.ServiceNotRegistered(typeof(TService));

            return null;
        }

        public void Add<TService>(IInitialContext<TService> context, bool error = true) where TService : class, IService
        {
            if (context == null)
                throw ThrowErrors.NullParameter(nameof(context));

            AddInternal(context, error);
        }

        public void Add<TService>(Func<TService> instanceProvider, bool error = true) where TService : class, IService
        {
            if (instanceProvider == null)
                throw ThrowErrors.NullParameter(nameof(instanceProvider));

            AddInternal(new DefaultInitialContext<TService>(instanceProvider), error);
        }

        private void AddInternal<TService>(IInitialContext<TService> context, bool error) where TService : class, IService
        {
            if (_storage.TryAdd(typeof(TService), new ServiceLocatorData(context)))
                return;

            if (error)
                throw new InvalidOperationException($"Service {typeof(TService)} already registered.");
        }

        private class DefaultInitialContext<TService> : IInitialContext<TService> where TService : class, IService
        {
            private Func<TService> _provider;

            public DefaultInitialContext(Func<TService> provider)
            {
                _provider = provider;
            }

            TService IInitialContext<TService>.GetOrCreateInstance()
            {
                return _provider.Invoke();
            }
        }
    }

    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException() : base() { }
        public ServiceNotFoundException(string message) : base(message) { }
        public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public ServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
