namespace OlegHcp.Managing
{
    public interface IService { }

    public interface IServiceLocator
    {
        TService Get<TService>() where TService : class, IService;
    }
}
