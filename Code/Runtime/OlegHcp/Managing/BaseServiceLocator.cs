using System;
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Tools;

namespace OlegHcp.Managing
{
    public interface IInitialContext
    {
        bool TryGetOrCreateInstance(Type serviceType, out IService service);
    }

    public class BaseServiceLocator : IServiceLocator
    {
        private protected IInitialContext Context;
        private protected Dictionary<Type, IService> ServiceCache = new Dictionary<Type, IService>();

        public BaseServiceLocator(IInitialContext context)
        {
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

            if (Context.TryGetOrCreateInstance(serviceType, out service))
                return (TService)ServiceCache.Place(serviceType, service);

            throw ThrowErrors.ServiceNotRegistered(typeof(TService));
        }
    }
}
