namespace OlegHcp.Managing
{
    public class DirtyServiceLocator : BoldServiceLocator
    {
        public DirtyServiceLocator(bool throwIfNotFound = true) : base(throwIfNotFound)
        {

        }

        public bool Remove<TService>() where TService : class, IService
        {
            bool contextRemoved = Contexts.Remove(typeof(TService));
            return RemoveInstance<TService>() || contextRemoved;
        }

        public void RemoveAll()
        {
            Contexts.Clear();
            RemoveAllInstances();
        }
    }
}
