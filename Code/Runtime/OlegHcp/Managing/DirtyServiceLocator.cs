namespace OlegHcp.Managing
{
    public class DirtyServiceLocator : BoldServiceLocator
    {
        public bool Remove<TService>(bool disposeIfPossible = true) where TService : class, IService
        {
            bool contextRemoved = Contexts.Remove(typeof(TService));
            return RemoveInstance<TService>(disposeIfPossible) || contextRemoved;
        }

        public void RemoveAll(bool disposeIfPossible = true)
        {
            Contexts.Clear();
            RemoveAllInstances(disposeIfPossible);
        }
    }
}
