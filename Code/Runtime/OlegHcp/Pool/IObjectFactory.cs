namespace OlegHcp.Pool
{
    public interface IObjectFactory<T> where T : class
    {
        T Create();
    }
}
