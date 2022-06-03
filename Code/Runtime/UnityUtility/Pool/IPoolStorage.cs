namespace UnityUtility.Pool
{
    public interface IPoolStorage<T> where T : class, IPoolable
    {
        int Count { get; }
        bool TryGet(out T value);
        bool TryAdd(T value);
        void Clear();
    }
}
