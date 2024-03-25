#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Managing
{
    public class DirtyServiceLocator : BoldServiceLocator
    {
        public bool Remove<TService>(bool dispose = true) where TService : class, IService
        {
            if (_storage.Remove(typeof(TService), out InitialContextData value))
            {
                value.ClearInstance(dispose);
                return true;
            }

            return false;
        }

        public void RemoveAll(bool dispose = true)
        {
            if (dispose)
            {
                foreach (InitialContextData value in _storage.Values)
                {
                    value.ClearInstance(true);
                }
            }

            _storage.Clear();
        }
    }
}
