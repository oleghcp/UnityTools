using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OlegHcp.Tools;

namespace OlegHcp.Managing
{
    public interface ICommonInitialContext
    {
        bool TryGetOrCreateInstance(Type serviceType, out IService service);
    }

    public interface IInitialContext<TService> where TService : class, IService
    {
        TService GetOrCreateInstance();
    }

    public class ServiceLocator : IServiceLocator
    {
        private protected InitialContextStorage _contextCache;
        private protected Dictionary<Type, IService> _serviceCache = new Dictionary<Type, IService>();

        public ServiceLocator()
        {
            _contextCache = new InitialContextStorage();
        }

        public ServiceLocator(ICommonInitialContext commonContext)
        {
            _contextCache = new InitialContextStorage(commonContext);
        }

        public TService Get<TService>(bool error = true) where TService : class, IService
        {
            Type serviceType = typeof(TService);

            if (_serviceCache.TryGetValue(serviceType, out IService service))
                return (TService)service;

            if (_contextCache.TryGetOrCreateInstance<TService>(out service))
            {
                _serviceCache.Add(serviceType, service);
                return (TService)service;
            }

            if (error)
                throw ThrowErrors.ServiceNotRegistered(typeof(TService));

            return null;
        }

        public void AddContext<TService>(IInitialContext<TService> context, bool error = true) where TService : class, IService
        {
            if (context == null)
                throw ThrowErrors.NullParameter(nameof(context));

            if (_contextCache.AddContext(context))
                return;

            if (error)
                throw new InvalidOperationException($"Service {typeof(TService)} already registered.");
        }

        public void AddContext<TService>(Func<TService> instanceProvider, bool error = true) where TService : class, IService
        {
            if (instanceProvider == null)
                throw ThrowErrors.NullParameter(nameof(instanceProvider));

            if (_contextCache.AddContext(new DefaultInitialContext<TService>(instanceProvider)))
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
