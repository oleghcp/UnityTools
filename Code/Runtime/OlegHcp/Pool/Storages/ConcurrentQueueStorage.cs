using System;
using System.Collections.Concurrent;
using OlegHcp.Pool;

namespace Assets.Code.Runtime.OlegHcp.Pool.Storages
{
    public class ConcurrentQueueStorage<T> : ConcurrentQueue<T>, IPoolStorage<T> where T : class
    {
        public bool TryAdd(T value)
        {
            Enqueue(value);
            return true;
        }

        public bool TryGet(out T value)
        {
            return TryDequeue(out value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
