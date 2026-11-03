using System.Collections.Generic;

namespace OlegHcp.Pool.Storages
{
    public class StackStorage<T> : Stack<T>, IPoolStorage<T> where T : class
    {
        public StackStorage() { }
        public StackStorage(int capacity) : base(capacity) { }
        public StackStorage(IEnumerable<T> collection) : base(collection) { }

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
