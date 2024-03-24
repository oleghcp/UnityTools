namespace OlegHcp.Managing
{
    public interface IService { }

    public interface IObjectFactory<out T> where T : class, IService
    {
        T Create();
    }
}
