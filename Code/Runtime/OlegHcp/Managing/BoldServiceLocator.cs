using System;

#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public class BoldServiceLocator : SimpleServiceLocator
    {
        public bool RemoveInstance<TService>(bool disposeIfPossible = true) where TService : class, IService
        {
            if (ServiceCache.Remove(typeof(TService), out IService service))
            {
                if (disposeIfPossible && service is IDisposable disposable)
                    disposable.Dispose();
                return true;
            }

            return false;
        }

        public void RemoveAllInstances(bool disposeIfPossible = true)
        {
            if (disposeIfPossible)
            {
                foreach (var (_, value) in ServiceCache)
                {
                    if (value is IDisposable disposable)
                        disposable.Dispose();
                }
            }

            ServiceCache.Clear();
        }
    }
}
