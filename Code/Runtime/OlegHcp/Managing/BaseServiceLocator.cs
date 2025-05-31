using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Managing
{
    public interface IInitialContext
    {
        bool TryCreateInstance(Type serviceType, out IService service);
    }

    public class BaseServiceLocator : IServiceLocator
    {
        private bool _throwIfNotFound;
        private protected IInitialContext Context;
        private protected Dictionary<Type, IService> ServiceCache = new Dictionary<Type, IService>();

        public bool ThrowIfNotFound => _throwIfNotFound;

        public BaseServiceLocator(IInitialContext context, bool throwIfNotFound = true)
        {
            _throwIfNotFound = throwIfNotFound;
            Context = context;
        }

        public TService Get<TService>() where TService : class, IService
        {
            return GetInternal<TService>();
        }

        private protected virtual TService GetInternal<TService>() where TService : class, IService
        {
            Type serviceType = typeof(TService);

            if (ServiceCache.TryGetValue(serviceType, out IService service))
                return (TService)service;

            if (Context.TryCreateInstance(serviceType, out service))
                return (TService)ServiceCache.Place(serviceType, service);

            if (_throwIfNotFound)
                throw new ServiceNotFoundException(typeof(TService).Name);

            return null;
        }
    }
}
