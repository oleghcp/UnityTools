using System.Collections.Generic;
#if !UNITY_2021_2_OR_NEWER
using OlegHcp.CSharp.Collections;
#endif

namespace OlegHcp.Pool.Storages
{
    public class StackStorage<T> : Stack<T>, IPoolStorage<T> where T : class
    {
        public StackStorage() { }
        public StackStorage(int capacity) : base(capacity) { }
        public StackStorage(IEnumerable<T> collection):base(collection) { }

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
