using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.Tools;

namespace OlegHcp.CSharp.Collections.Iterators
{
    public struct Enumerator_<T> : IEnumerator<T>
    {
        private readonly IReadOnlyList<T> _collection;
        private readonly int _version;
        private readonly int _count;

        private int _index;
        private T _current;

        public T Current => _current;

        object IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _count + 1)
                    throw new InvalidOperationException();
                return _current;
            }
        }

        public Enumerator_(IReadOnlyList<T> collection, int startIndex, int length)
        {
            if ((uint)startIndex >= (uint)collection.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + length > collection.Count)
                throw new ArgumentOutOfRangeException(nameof(length));

            _collection = collection;
            _index = startIndex;
            _count = startIndex + length;
            _current = default;

            InitVersion(collection, out _version);
        }

        public Enumerator_(IReadOnlyList<T> collection, int startIndex)
        {
            if ((uint)startIndex >= (uint)collection.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            _collection = collection;
            _index = startIndex;
            _count = collection.Count;
            _current = default;

            InitVersion(collection, out _version);
        }

        public Enumerator_(IReadOnlyList<T> collection)
        {
            _collection = collection;
            _count = collection.Count;
            _index = 0;
            _current = default;

            InitVersion(collection, out _version);
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (Changed())
                throw ThrowErrors.CollectionChanged();

            if ((uint)_index < (uint)_count)
            {
                _current = _collection[_index];
                _index++;
                return true;
            }

            _index = _count + 1;
            _current = default;
            return false;
        }

        void IEnumerator.Reset()
        {
            if (Changed())
                throw ThrowErrors.CollectionChanged();

            _index = 0;
            _current = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Changed()
        {
            return _collection is IMutable mutable && mutable.Version != _version;
        }

        private static void InitVersion(IReadOnlyList<T> collection, out int version)
        {
            if (collection is IMutable mutable)
                version = mutable.Version;
            else
                version = default;
        }
    }
}
