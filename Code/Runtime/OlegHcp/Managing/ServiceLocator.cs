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

    public interface IInitialContext<out T> where T : class, IService
    {
        T GetOrCreateInstance();
    }

    public class ServiceLocator
    {
        private protected Dictionary<Type, Data> _storage = new Dictionary<Type, Data>();

        public T Get<T>(bool error = true) where T : class, IService
        {
            if (_storage.TryGetValue(typeof(T), out Data value))
                return (T)value.Service;

            if (error)
                throw ThrowErrors.ServiceNotRegistered(typeof(T));

            return null;
        }

        public void Register<T>(IInitialContext<T> context, bool error = true) where T : class, IService
        {
            if (context == null)
                throw ThrowErrors.NullParameter(nameof(context));

            if (_storage.TryAdd(typeof(T), new Data(context)))
                return;

            if (error)
                throw new InvalidOperationException($"Service {typeof(T)} already registered.");
        }

        public void Register<T>(Func<T> instanceProvider, bool error = true) where T : class, IService
        {
            if (instanceProvider == null)
                throw ThrowErrors.NullParameter(nameof(instanceProvider));

            Register(new DefaultInitialContext<T>(instanceProvider), error);
        }

        private protected class Data
        {
            private IService _service;
            private IInitialContext<IService> _context;

            public IService Service => _service ?? (_service = _context.GetOrCreateInstance());

            public Data(IInitialContext<IService> context)
            {
                _context = context;
            }

            public void Clear(bool dispose)
            {
                if (dispose && _service is IDisposable disposable)
                    disposable.Dispose();

                _service = null;
            }
        }

        private class DefaultInitialContext<T> : IInitialContext<T> where T : class, IService
        {
            private Func<T> _provider;

            public DefaultInitialContext(Func<T> provider)
            {
                _provider = provider;
            }

            T IInitialContext<T>.GetOrCreateInstance()
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
