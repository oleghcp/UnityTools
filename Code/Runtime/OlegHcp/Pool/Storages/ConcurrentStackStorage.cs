using System.Collections.Concurrent;
using OlegHcp.Pool;

namespace Runtime.OlegHcp.Pool.Storages
{
    public class ConcurrentStackStorage<T> : ConcurrentStack<T>, IPoolStorage<T> where T : class
    {
        public bool TryAdd(T value)
        {
            Push(value);
            return true;
        }

        public bool TryGet(out T value)
        {
            return TryPop(out value);
        }
    }
}
