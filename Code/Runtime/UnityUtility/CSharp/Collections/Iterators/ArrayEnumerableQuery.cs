#if !UNITY_2021_2_OR_NEWER
using System.Collections;
using System.Collections.Generic;

namespace OlegHcp.CSharp.Collections.Iterators
{
    public readonly struct ArrayEnumerableQuery<T> : IEnumerable<T>
    {
        private readonly ArrayEnumerator<T> _enumerator;

        public ArrayEnumerableQuery(T[] array, int startIndex, int length, IMutable versionProvider = null)
        {
            _enumerator = new ArrayEnumerator<T>(array, startIndex, length, versionProvider);
        }

        public ArrayEnumerableQuery(T[] array, int startIndex, IMutable versionProvider = null)
        {
            _enumerator = new ArrayEnumerator<T>(array, startIndex, versionProvider);
        }

        public ArrayEnumerableQuery(T[] array, IMutable versionProvider = null)
        {
            _enumerator = new ArrayEnumerator<T>(array, versionProvider);
        }

        public ArrayEnumerator<T> GetEnumerator()
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
