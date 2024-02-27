using System.Collections.Generic;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.Pool.Storages
{
    public class StackStorage<T> : Stack<T>, IPoolStorage<T> where T : class, IPoolable
    {
        public StackStorage()
        {

        }

        public StackStorage(int capacity) : base(capacity)
        {

        }

        public bool TryAdd(T value)
        {
            Push(value);
            return true;
        }

        public bool TryGet(out T value)
        {
#if UNITY_2021_2_OR_NEWER
            return TryPop(out value);
#else
            return this.TryPop(out value);
#endif
        }
    }
}
