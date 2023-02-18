using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility.Tools;

namespace UnityUtility.CSharp.Collections.Iterators
{
    public struct ArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _array;
        private readonly IMutable _versionProvider;
        private readonly int _version;

        private int _index;
        private int _count;
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

        public ArrayEnumerator(T[] array, int startIndex, int length, IMutable versionProvider = null)
        {
            if ((uint)startIndex >= (uint)array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            _array = array;
            _index = startIndex;
            _count = startIndex + length;
            _current = default;

            InitVersion(versionProvider, out _versionProvider, out _version);
        }

        public ArrayEnumerator(T[] array, int startIndex, IMutable versionProvider = null)
        {
            if ((uint)startIndex >= (uint)array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            _array = array;
            _index = startIndex;
            _count = array.Length;
            _current = default;

            InitVersion(versionProvider, out _versionProvider, out _version);
        }

        public ArrayEnumerator(T[] array, IMutable versionProvider = null)
        {
            _array = array;
            _index = 0;
            _count = array.Length;
            _current = default;

            InitVersion(versionProvider, out _versionProvider, out _version);
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
                _current = _array[_index];
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
            return _versionProvider != null && _versionProvider.Version != _version;
        }

        private static void InitVersion(IMutable versionProvider, out IMutable versionProviderField, out int version)
        {
            if (versionProvider != null)
            {
                version = versionProvider.Version;
                versionProviderField = versionProvider;
            }
            else
            {
                version = default;
                versionProviderField = null;
            }
        }
    }
}
