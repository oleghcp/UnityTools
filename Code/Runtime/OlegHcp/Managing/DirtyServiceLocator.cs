namespace OlegHcp.Managing
{
    public class DirtyServiceLocator : BoldServiceLocator
    {
        public bool Remove<TService>(bool dispose = true) where TService : class, IService
        {
            _contextCache.RemoveContext<TService>();
            return RemoveInstance<TService>(dispose);
        }

        public void RemoveAll(bool dispose = true)
        {
            _contextCache.Clear();
            RemoveAllInstances(dispose);
        }
    }
}
