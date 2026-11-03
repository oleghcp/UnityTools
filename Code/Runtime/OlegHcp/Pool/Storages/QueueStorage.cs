using System.Collections.Generic;

namespace OlegHcp.Pool.Storages
{
    public class QueueStorage<T> : Queue<T>, IPoolStorage<T> where T : class
    {
        public QueueStorage() { }
        public QueueStorage(int capacity) : base(capacity) { }
        public QueueStorage(IEnumerable<T> collection) : base(collection) { }

        public bool TryAdd(T value)
        {
            Enqueue(value);
            return true;
        }

        public bool TryGet(out T value)
        {
            return TryDequeue(out value);
        }
    }
}
