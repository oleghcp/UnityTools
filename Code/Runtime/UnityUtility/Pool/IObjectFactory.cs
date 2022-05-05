namespace UnityUtility.Pool
{
    public interface IObjectFactory<T> where T : class, IPoolable
    {
        T Create();
    }
}
