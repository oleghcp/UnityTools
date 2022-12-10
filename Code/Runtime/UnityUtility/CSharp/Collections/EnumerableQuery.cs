using System.Collections;
using System.Collections.Generic;

namespace UnityUtility.CSharp.Collections
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
