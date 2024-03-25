namespace OlegHcp.Managing
{
    public class BoldServiceLocator : ServiceLocator
    {
        public bool RemoveInstance<TService>(bool dispose = true) where TService : class, IService
        {
            if (_storage.TryGetValue(typeof(TService), out ServiceLocatorData value))
            {
                value.ClearInstance(dispose);
                return true;
            }

            return false;
        }

        public void RemoveAllInstances(bool dispose = true)
        {
            foreach (ServiceLocatorData item in _storage.Values)
            {
                item.ClearInstance(dispose);
            }
        }
    }
}
