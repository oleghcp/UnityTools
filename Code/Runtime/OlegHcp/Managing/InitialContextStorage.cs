using System;
using System.Collections.Generic;

#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    internal class InitialContextStorage
    {
        private ICommonInitialContext _commonContext;
        private protected Dictionary<Type, object> _contextCache = new Dictionary<Type, object>();

        public InitialContextStorage()
        {
            _commonContext = new DefaultInitialContext();
        }

        public InitialContextStorage(ICommonInitialContext commonContext)
        {
            _commonContext = commonContext;
        }

        public bool TryGetOrCreateInstance<TService>(out TService service) where TService : class, IService
        {
            if (_contextCache.TryGetValue(typeof(TService), out object value))
            {
                IInitialContext<TService> context = (IInitialContext<TService>)value;
                service = context.GetOrCreateInstance();
                return true;
            }

            return _commonContext.TryGetOrCreateInstance(out service);
        }

        public bool AddContext<TService>(IInitialContext<TService> context) where TService : class, IService
        {
            return _contextCache.TryAdd(typeof(TService), context);
        }

        public bool RemoveContext<TService>() where TService : class, IService
        {
            return _contextCache.Remove(typeof(TService));
        }

        public void Clear()
        {
            _contextCache.Clear();
        }

        private class DefaultInitialContext : ICommonInitialContext
        {
            bool ICommonInitialContext.TryGetOrCreateInstance<TService>(out TService service)
            {
                service = default;
                return false;
            }
        }
    }
}
