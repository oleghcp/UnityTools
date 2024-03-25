#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public class ServiceLocatorDirty : ServiceLocator
    {
        public bool RemoveInstance<T>(bool dispose = true) where T : class, IService
        {
            if (_storage.TryGetValue(typeof(T), out Data value))
            {
                value.Clear(dispose);
                return true;
            }

            return false;
        }

        public void ClearInstances(bool dispose = true)
        {
            foreach (Data item in _storage.Values)
            {
                item.Clear(dispose);
            }
        }

        public bool Unregister<T>(bool dispose = true) where T : class, IService
        {
            if (_storage.Remove(typeof(T), out Data value))
            {
                value.Clear(dispose);
                return true;
            }

            return false;
        }

        public void UnregisterAll(bool dispose = true)
        {
            if (dispose)
            {
                foreach (Data value in _storage.Values)
                {
                    value.Clear(true);
                }
            }

            _storage.Clear();
        }
    }
}
