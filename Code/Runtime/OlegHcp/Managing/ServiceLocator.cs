using System;
using System.Collections.Generic;
using OlegHcp.Tools;

#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public class ServiceLocator
    {
        private Dictionary<Type, IService> _storage = new Dictionary<Type, IService>();

        public T Get<T>(bool error = true) where T : class, IService
        {
            if (_storage.TryGetValue(typeof(T), out IService value))
                return (T)value;

            if (error)
                throw ThrowErrors.ServiceNotRegistered(typeof(T));

            return null;
        }

        public void Register<T>(T service, bool error = true) where T : class, IService
        {
            if (service == null)
                throw ThrowErrors.NullParameter(nameof(service));

            if (_storage.TryAdd(typeof(T), service))
                return;

            if (error)
                throw new InvalidOperationException($"Service {typeof(T).Name} already registered.");
        }

        public bool Unregister<T>(bool dispose = true) where T : class, IService
        {
            if (!dispose)
                return _storage.Remove(typeof(T));

            if (_storage.Remove(typeof(T), out IService value))
            {
                if (value is IDisposable disposable)
                    disposable.Dispose();

                return true;
            }

            return false;
        }

        public void UnregisterAll(bool dispose = true)
        {
            if (dispose)
            {
                foreach (var value in _storage.Values)
                {
                    if (value is IDisposable disposable)
                        disposable.Dispose();
                }
            }

            _storage.Clear();
        }
    }
}
