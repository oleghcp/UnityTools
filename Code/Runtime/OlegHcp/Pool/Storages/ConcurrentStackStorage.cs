using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OlegHcp.Pool.Storages
{
    public class ConcurrentStackStorage<T> : ConcurrentStack<T>, IPoolStorage<T> where T : class
    {
        public ConcurrentStackStorage() { }
        public ConcurrentStackStorage(IEnumerable<T> collection) : base(collection) { }

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
