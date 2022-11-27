////////////////////////////////////////////////////////////
// Analogue of real System.Span`1 from .net core library. //
////////////////////////////////////////////////////////////

#if !UNITY_2021_2_OR_NEWER
using System.ComponentModel;
using UnityUtility.Tools;

namespace System
{
#pragma warning disable CS0809
    public unsafe ref struct Span<T> where T : unmanaged
    {
        private readonly T* _ptr;
        private readonly int _length;

        public bool IsEmpty => _length == 0;
        public int Length => _length;

        internal T* Ptr => _ptr;

        public ref T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_length)
                    throw new IndexOutOfRangeException();

                if (_ptr == null)
                    throw new NullReferenceException("Span pointer is null.");

                return ref _ptr[index];
            }
        }

        public Span(void* ptr, int length)
        {
            if (length < 0)
                throw Errors.NegativeParameter(nameof(length));

            _ptr = (T*)ptr;
            _length = ptr == null ? 0 : length;
        }

        public void Clear()
        {
            for (int i = 0; i < _length; i++)
            {
                _ptr[i] = default;
            }
        }

        public void Fill(T value)
        {
            for (int i = 0; i < _length; i++)
            {
                _ptr[i] = value;
            }
        }

        public T[] ToArray()
        {
            T[] newArray = new T[_length];

            for (int i = 0; i < _length; i++)
            {
                newArray[i] = _ptr[i];
            }

            return newArray;
        }

        public Span<T> Slice(int startIndex, int length)
        {
            if ((uint)startIndex >= (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + length > _length)
                throw new ArgumentOutOfRangeException(nameof(length));

            return new Span<T>(_ptr + startIndex, length);
        }

        public Span<T> Slice(int startIndex)
        {
            if ((uint)startIndex >= (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return new Span<T>(_ptr + startIndex, _length - startIndex);
        }

#region Regular stuff
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        [Obsolete("Equals() on Span will always throw an exception. Use == instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            throw new NotSupportedException("Cannot call equals on Span.");
        }

        [Obsolete("GetHashCode() on Span will always throw an exception.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            throw new NotSupportedException("Cannot call GetHashCode() on Span.");
        }

        public override string ToString()
        {
            if (typeof(T) == typeof(char))
                return new string((char*)_ptr);

            return $"System.Span<{typeof(T).Name}>[{_length}]";
        }

        public static bool operator ==(Span<T> left, Span<T> right)
        {
            if (left._length == right._length)
                return left._ptr == right._ptr;

            return false;
        }

        public static bool operator !=(Span<T> left, Span<T> right)
        {
            return !(left == right);
        }
#endregion

        public ref struct Enumerator
        {
            private readonly Span<T> _span;

            private int _index;

            public ref T Current => ref _span[_index];

            internal Enumerator(Span<T> span)
            {
                _span = span;
                _index = -1;
            }

            public bool MoveNext()
            {
                int num = _index + 1;
                if (num < _span.Length)
                {
                    _index = num;
                    return true;
                }
                return false;
            }
        }
    }
}
#endif
