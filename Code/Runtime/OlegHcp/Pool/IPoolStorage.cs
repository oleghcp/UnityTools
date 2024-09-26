namespace OlegHcp.Pool
{
    public interface IPoolStorage<T> where T : class
    {
        int Count { get; }
        bool TryGet(out T value);
        bool TryAdd(T value);
        void Clear();
    }
}
