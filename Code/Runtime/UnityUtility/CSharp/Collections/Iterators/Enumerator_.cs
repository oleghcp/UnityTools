using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility.Tools;

namespace UnityUtility.CSharp.Collections.Iterators
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

            if (collection is IMutable mutable)
                _version = mutable.Version;
            else
                _version = default;
        }

        public Enumerator_(IReadOnlyList<T> collection, int startIndex)
        {
            if ((uint)startIndex >= (uint)collection.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            _collection = collection;
            _index = startIndex;
            _count = collection.Count;
            _current = default;

            if (collection is IMutable mutable)
                _version = mutable.Version;
            else
                _version = default;
        }

        public Enumerator_(IReadOnlyList<T> collection)
        {
            _collection = collection;
            _count = collection.Count;
            _index = 0;
            _current = default;

            if (collection is IMutable mutable)
                _version = mutable.Version;
            else
                _version = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (Changed())
                throw Errors.CollectionChanged();

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
                throw Errors.CollectionChanged();

            _index = 0;
            _current = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Changed()
        {
            return _collection is IMutable mutable && mutable.Version != _version;
        }
    }
}
