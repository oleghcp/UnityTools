using System;
using System.Collections.Generic;
using OlegHcp.Tools;

namespace OlegHcp.Managing
{
    public interface IInitialContext
    {
        bool TryGetOrCreateInstance(Type serviceType, out IService service);
    }

    public class BaseServiceLocator : IServiceLocator
    {
        private protected IInitialContext _context;
        private protected Dictionary<Type, IService> _serviceCache = new Dictionary<Type, IService>();

        public BaseServiceLocator(IInitialContext context)
        {
            _context = context;
        }

        public TService Get<TService>(bool throwIfNotFound = true) where TService : class, IService
        {
            Type serviceType = typeof(TService);

            if (_serviceCache.TryGetValue(serviceType, out IService service))
                return (TService)service;

            if (_context.TryGetOrCreateInstance(serviceType, out service))
            {
                _serviceCache.Add(serviceType, service);
                return (TService)service;
            }

            if (throwIfNotFound)
                throw ThrowErrors.ServiceNotRegistered(typeof(TService));

            return null;
        }
    }
}
