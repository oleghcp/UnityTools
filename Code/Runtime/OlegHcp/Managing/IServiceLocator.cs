namespace OlegHcp.Managing
{
    public interface IService { }

    public interface IServiceLocator
    {
        TService Get<TService>(bool error = true) where TService : class, IService;
    }
}
