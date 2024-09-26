using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OlegHcp.Pool.Storages
{
    public class ConcurrentQueueStorage<T> : ConcurrentQueue<T>, IPoolStorage<T> where T : class
    {
        public ConcurrentQueueStorage() { }
        public ConcurrentQueueStorage(IEnumerable<T> collection) : base(collection) { }

        public bool TryAdd(T value)
        {
            Enqueue(value);
            return true;
        }

        public bool TryGet(out T value)
        {
            return TryDequeue(out value);
        }

#if !UNITY_2021_2_OR_NEWER
        public void Clear()
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}
