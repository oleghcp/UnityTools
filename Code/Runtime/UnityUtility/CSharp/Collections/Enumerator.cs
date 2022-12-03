using System;
using System.Collections;
using System.Collections.Generic;
using UnityUtility.Tools;

namespace UnityUtility.CSharp.Collections
{
    public struct Enumerator<T> : IEnumerator<T>
    {
        private readonly IList<T> _collection;
        private readonly int _version;

        private int _index;
        private T _current;

        public T Current => _current;

        object IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _collection.Count + 1)
                    throw new InvalidOperationException();
                return _current;
            }
        }

        public Enumerator(IList<T> collection)
        {
            _collection = collection;
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

            if ((uint)_index < (uint)_collection.Count)
            {
                _current = _collection[_index];
                _index++;
                return true;
            }

            _index = _collection.Count + 1;
            _current = default;
            return false;
        }

        private bool Changed()
        {
            return _collection is IMutable mutable && mutable.Version != _version;
        }

        void IEnumerator.Reset()
        {
            if (Changed())
                throw Errors.CollectionChanged();

            _index = 0;
            _current = default;
        }
    }
}
