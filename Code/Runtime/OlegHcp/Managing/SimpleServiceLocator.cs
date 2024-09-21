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
        private protected Dictionary<Type, object> Contexts = new Dictionary<Type, object>();
        private protected Dictionary<Type, IService> ServiceCache = new Dictionary<Type, IService>();
        private bool _throwIfNotFound;

        public SimpleServiceLocator(bool throwIfNotFound = true)
        {
            _throwIfNotFound = throwIfNotFound;
        }

        public TService Get<TService>() where TService : class, IService
        {
            Type serviceType = typeof(TService);

            if (ServiceCache.TryGetValue(serviceType, out IService service))
                return (TService)service;

            if (Contexts.TryGetValue(serviceType, out object value))
            {
                IInitialContext<TService> context = value as IInitialContext<TService>;
                TService newService = context.GetOrCreateInstance();
                ServiceCache.Add(serviceType, newService);
                return newService;
            }

            if (_throwIfNotFound)
                throw ThrowErrors.ServiceNotRegistered(typeof(TService));

            return null;
        }

        public bool ContainsContext<TService>() where TService : class, IService
        {
            return Contexts.ContainsKey(typeof(TService));
        }

        public void AddContext<TService>(IInitialContext<TService> context) where TService : class, IService
        {
            if (context == null)
                throw ThrowErrors.NullParameter(nameof(context));

            if (Contexts.TryAdd(typeof(TService), context))
                return;

            throw new InvalidOperationException($"Service {typeof(TService)} already registered.");
        }

        public void AddContext<TService>(Func<TService> instanceProvider) where TService : class, IService
        {
            if (instanceProvider == null)
                throw ThrowErrors.NullParameter(nameof(instanceProvider));

            if (Contexts.TryAdd(typeof(TService), new DefaultInitialContext<TService>(instanceProvider)))
                return;

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
