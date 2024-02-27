#if !UNITY_2021_2_OR_NEWER
using System.Collections;
using System.Collections.Generic;

namespace OlegHcp.CSharp.Collections.Iterators
{
    public readonly struct EnumerableQuery_<T> : IEnumerable<T>
    {
        private readonly Enumerator_<T> _enumerator;

        public EnumerableQuery_(IReadOnlyList<T> collection, int startIndex, int length)
        {
            _enumerator = new Enumerator_<T>(collection, startIndex, length);
        }

        public EnumerableQuery_(IReadOnlyList<T> collection, int startIndex)
        {
            _enumerator = new Enumerator_<T>(collection, startIndex);
        }

        public EnumerableQuery_(IReadOnlyList<T> collection)
        {
            _enumerator = new Enumerator_<T>(collection);
        }

        public Enumerator_<T> GetEnumerator()
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
