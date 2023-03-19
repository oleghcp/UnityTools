#if UNITY_2021_2_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityUtility.CSharp.Collections.Iterators;
using UnityUtility.Tools;

namespace UnityUtility.CSharp.Collections.ReadOnly
{
    public readonly struct ReadOnlySegment<T> : IReadOnlyList<T>, IEquatable<ReadOnlySegment<T>>
    {
        private readonly IReadOnlyList<T> _items;
        private readonly int _offset;
        private readonly int _count;

        public T this[int index] => _items[_offset + index];

        public ReadOnlySegment<T> this[Range range]
        {
            get
            {
                (int offset, int length) = range.GetOffsetAndLength(_count);
                return new ReadOnlySegment<T>(_items, _offset + offset, length);
            }
        }

        public IReadOnlyList<T> Items => _items;
        public int Offset => _offset;
        public int Count => _count;

        public ReadOnlySegment(IReadOnlyList<T> items) : this(items, 0, items.Count) { }

        public ReadOnlySegment(IReadOnlyList<T> items, int offset) : this(items, offset, items.Count - offset) { }

        public ReadOnlySegment(IReadOnlyList<T> items, int offset, int count)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count < offset + count)
                throw ThrowErrors.SegmentOutOfRange();

            _items = items;
            _offset = offset;
            _count = count;
        }

        public int IndexOf(T item)
        {
            for (int i = _offset; i < _offset + _count; i++)
            {
                if (Items[i].Equals(item))
                    return i;
            }

            return -1;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] destination)
        {
            CopyTo(destination, 0);
        }

        public void CopyTo(T[] destination, int destinationIndex)
        {
            if (_count > destination.Length)
                throw ThrowErrors.DestinationTooShort(nameof(destination));

            if ((uint)destinationIndex >= (uint)destination.Length)
                throw ThrowErrors.IndexOutOfRange();

            for (int i = _offset; i < _offset + _count; i++)
            {
                destination[destinationIndex++] = _items[i];
            }
        }

        public void CopyTo(IList<T> destination)
        {
            CopyTo(destination, 0);
        }

        public void CopyTo(IList<T> destination, int destinationIndex)
        {
            if (_count > destination.Count)
                throw ThrowErrors.DestinationTooShort(nameof(destination));

            if ((uint)destinationIndex >= (uint)destination.Count)
                throw ThrowErrors.IndexOutOfRange();

            for (int i = _offset; i < _offset + _count; i++)
            {
                destination[destinationIndex++] = _items[i];
            }
        }

        public ReadOnlySegment<T> Slice(int index)
        {
            return new ReadOnlySegment<T>(_items, _offset + index, _count - index);
        }

        public ReadOnlySegment<T> Slice(int index, int count)
        {
            return new ReadOnlySegment<T>(_items, _offset + index, count);
        }

        public T[] ToArray()
        {
            if (_count == 0)
                return Array.Empty<T>();

            T[] subArray = new T[_count];
            for (int i = 0; i < _count; i++)
            {
                subArray[i] = _items[i + _offset];
            }
            return subArray;
        }

        public Enumerator_<T> GetEnumerator()
        {
            return new Enumerator_<T>(_items, _offset, _count);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator_<T>(_items, _offset, _count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator_<T>(_items, _offset, _count);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_offset, _count, _items.GetHashCode());
        }

        public bool Equals(ReadOnlySegment<T> other)
        {
            if (other._items == _items && other._offset == _offset)
                return other._count == _count;

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is ReadOnlySegment<T> segment && Equals(segment);
        }

        public static implicit operator ReadOnlySegment<T>(List<T> list)
        {
            return new ReadOnlySegment<T>(list);
        }

        public static bool operator ==(ReadOnlySegment<T> a, ReadOnlySegment<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ReadOnlySegment<T> a, ReadOnlySegment<T> b)
        {
            return !a.Equals(b);
        }
    }
}
#endif
