using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Pool.Storages
{
    public class QueueStorage<T> : Queue<T>, IPoolStorage<T> where T : class, IPoolable
    {
        public QueueStorage()
        {

        }

        public QueueStorage(int capacity) : base(capacity)
        {

        }

        public bool TryAdd(T value)
        {
            Enqueue(value);
            return true;
        }

        public bool TryGet(out T value)
        {
#if UNITY_2021_2_OR_NEWER
            return TryDequeue(out value);
#else
            return this.TryDequeue(out value);
#endif
        }
    }
}
