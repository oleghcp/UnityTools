#if !UNITY_2021_2_OR_NEWER
using System.Collections;
using System.Collections.Generic;

namespace OlegHcp.CSharp.Collections.Iterators
{
    public readonly struct EnumerableQuery<T> : IEnumerable<T>
    {
        private readonly Enumerator<T> _enumerator;

        public EnumerableQuery(IList<T> collection, int startIndex, int length)
        {
            _enumerator = new Enumerator<T>(collection, startIndex, length);
        }

        public EnumerableQuery(IList<T> collection, int startIndex)
        {
            _enumerator = new Enumerator<T>(collection, startIndex);
        }

        public EnumerableQuery(IList<T> collection)
        {
            _enumerator = new Enumerator<T>(collection);
        }

        public Enumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }
}
#endif
