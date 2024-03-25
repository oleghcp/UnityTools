namespace OlegHcp.Managing
{
    public class DirtyServiceLocator : BoldServiceLocator
    {
        public bool Remove<TService>(bool disposeIfPossible = true) where TService : class, IService
        {
            bool contextRemoved = _contexts.Remove(typeof(TService));
            return RemoveInstance<TService>(disposeIfPossible) || contextRemoved;
        }
    }
}
