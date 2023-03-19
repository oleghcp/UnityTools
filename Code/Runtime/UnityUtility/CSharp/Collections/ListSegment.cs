#if UNITY_2021_2_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityUtility.CSharp.Collections.Iterators;
using UnityUtility.Tools;

namespace UnityUtility.CSharp.Collections
{
    public readonly struct ListSegment<T> : IList<T>, IEquatable<ListSegment<T>>
    {
        private readonly IList<T> _items;
        private readonly int _offset;
        private readonly int _count;

        public T this[int index]
        {
            get => _items[_offset + index];
            set => _items[_offset + index] = value;
        }

        //public ListSegment<T> this[Range range] => throw new NotImplementedException();

        public IList<T> Items => _items;
        public int Offset => _offset;
        public int Count => _count;
        bool ICollection<T>.IsReadOnly => false;

        public ListSegment(IList<T> items) : this(items, 0, items.Count) { }

        public ListSegment(IList<T> items, int offset) : this(items, offset, items.Count - offset) { }

        public ListSegment(IList<T> items, int offset, int count)
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

        public void CopyTo(ListSegment<T> destination)
        {
            CopyTo(destination.Items, destination.Offset);
        }

        public ListSegment<T> Slice(int index)
        {
            return new ListSegment<T>(_items, _offset + index, _count - index);
        }

        public ListSegment<T> Slice(int index, int count)
        {
            return new ListSegment<T>(_items, _offset + index, count);
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

        #region Not Implemented
        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }
        #endregion

        public Enumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(_items, _offset, _count);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator<T>(_items, _offset, _count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<T>(_items, _offset, _count);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_offset, _count, _items.GetHashCode());
        }

        public bool Equals(ListSegment<T> other)
        {
            if (other._items == _items && other._offset == _offset)
                return other._count == _count;

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is ListSegment<T> segment && Equals(segment);
        }

        public static implicit operator ListSegment<T>(List<T> list)
        {
            return new ListSegment<T>(list);
        }

        public static bool operator ==(ListSegment<T> a, ListSegment<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ListSegment<T> a, ListSegment<T> b)
        {
            return !a.Equals(b);
        }
    }
}
#endif
