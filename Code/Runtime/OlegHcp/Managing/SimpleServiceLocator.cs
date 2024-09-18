using System;
using System.Collections.Generic;
using OlegHcp.Tools;

#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public interface IInitialContext<TService> where TService : class, IService
    {
        TService GetOrCreateInstance();
    }

    public class SimpleServiceLocator : IServiceLocator
    {
        private Dictionary<Type, object> _contexts = new Dictionary<Type, object>();
        private Dictionary<Type, IService> _serviceCache = new Dictionary<Type, IService>();

        private protected Dictionary<Type, object> Contexts => _contexts;
        private protected Dictionary<Type, IService> ServiceCache => _serviceCache;

        public TService Get<TService>(bool throwIfNotFound = true) where TService : class, IService
        {
            Type serviceType = typeof(TService);

            if (_serviceCache.TryGetValue(serviceType, out IService service))
                return (TService)service;

            if (_contexts.TryGetValue(serviceType, out object value))
            {
                IInitialContext<TService> context = value as IInitialContext<TService>;
                TService newService = context.GetOrCreateInstance();
                _serviceCache.Add(serviceType, newService);
                return newService;
            }

            if (throwIfNotFound)
                throw ThrowErrors.ServiceNotRegistered(typeof(TService));

            return null;
        }

        public void AddContext<TService>(IInitialContext<TService> context, bool throwIfContains = true) where TService : class, IService
        {
            if (context == null)
                throw ThrowErrors.NullParameter(nameof(context));

            if (_contexts.TryAdd(typeof(TService), context))
                return;

            if (throwIfContains)
                throw new InvalidOperationException($"Service {typeof(TService)} already registered.");
        }

        public void AddContext<TService>(Func<TService> instanceProvider, bool throwIfContains = true) where TService : class, IService
        {
            if (instanceProvider == null)
                throw ThrowErrors.NullParameter(nameof(instanceProvider));

            if (_contexts.TryAdd(typeof(TService), new DefaultInitialContext<TService>(instanceProvider)))
                return;

            if (throwIfContains)
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
}
