namespace OlegHcp.Managing
{
    public interface IService { }

    public interface IServiceLocator
    {
        TService Get<TService>(bool throwIfNotFound = true) where TService : class, IService;
    }
}
