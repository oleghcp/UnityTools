namespace OlegHcp.Managing
{
    public class BoldServiceLocator : ServiceLocator
    {
        public bool RemoveInstance<TService>(bool dispose = true) where TService : class, IService
        {
            if (_storage.TryGetValue(typeof(TService), out InitialContextData value))
            {
                value.ClearInstance(dispose);
                return true;
            }

            return false;
        }

        public void RemoveAllInstances(bool dispose = true)
        {
            foreach (InitialContextData item in _storage.Values)
            {
                item.ClearInstance(dispose);
            }
        }
    }
}
