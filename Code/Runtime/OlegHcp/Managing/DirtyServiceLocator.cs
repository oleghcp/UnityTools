namespace OlegHcp.Managing
{
    public class DirtyServiceLocator : BoldServiceLocator
    {
        public DirtyServiceLocator() { }

        public DirtyServiceLocator(ICommonInitialContext commonContext) : base(commonContext) { }

        public bool Remove<TService>(bool disposeIfPossible = true) where TService : class, IService
        {
            _contextCache.RemoveContext<TService>();
            return RemoveInstance<TService>(disposeIfPossible);
        }

        public void RemoveAll(bool disposeIfPossible = true)
        {
            _contextCache.Clear();
            RemoveAllInstances(disposeIfPossible);
        }
    }
}
