using System;

namespace OlegHcp.Managing
{
    public class BoldServiceLocator : SimpleServiceLocator
    {
        public BoldServiceLocator(bool throwIfNotFound = true) : base(throwIfNotFound)
        {

        }

        public bool RemoveInstance<TService>() where TService : class, IService
        {
            if (ServiceCache.Remove(typeof(TService), out IService service))
            {
                if (service is IDisposable disposable)
                    disposable.Dispose();
                return true;
            }

            return false;
        }

        public void RemoveAllInstances()
        {
            foreach (var (_, value) in ServiceCache)
            {
                if (value is IDisposable disposable)
                    disposable.Dispose();
            }

            ServiceCache.Clear();
        }
    }
}
