using System;

#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public class BoldServiceLocator : ServiceLocator
    {
        public BoldServiceLocator() { }

        public BoldServiceLocator(ICommonInitialContext commonContext) : base(commonContext) { }

        public bool RemoveInstance<TService>(bool dispose = true) where TService : class, IService
        {
            if (_serviceCache.Remove(typeof(TService), out IService service))
            {
                if (dispose && service is IDisposable disposable)
                    disposable.Dispose();
                return true;
            }

            return false;
        }

        public void RemoveAllInstances(bool dispose = true)
        {
            if (dispose)
            {
                foreach (IService service in _serviceCache.Values)
                {
                    if (service is IDisposable disposable)
                        disposable.Dispose();
                }
            }

            _serviceCache.Clear();
        }
    }
}
