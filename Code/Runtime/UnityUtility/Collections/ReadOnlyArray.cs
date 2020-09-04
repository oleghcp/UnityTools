using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnityUtility.Collections
{
    public struct ReadOnlyArray<T> : IList<T>, IReadOnlyList<T>, IEquatable<ReadOnlyArray<T>>
    {
        private IList<T> m_list;

        public T this[int index]
        {
            get { return m_list[index]; }
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ReadOnlyArray(IList<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            m_list = collection;
        }

        public bool Contains(T item)
        {
            return m_list.Contains(item);
        }

        public int IndexOf(T item)
        {
            return m_list.IndexOf(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_list.CopyTo(array, arrayIndex);
        }

        public T GetLast()
        {
            return m_list.GetLast();
        }

        public ReadOnlyCollection<T> ToCollection()
        {
            return new ReadOnlyCollection<T>(m_list);
        }

        public T[] ToArray()
        {
            return m_list.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        #region Not Supported
        T IList<T>.this[int index]
        {
            get { return m_list[index]; }
            set { throw new NotSupportedException(); }
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }
        #endregion

        // -- //

        public override bool Equals(object obj)
        {
            return obj is ReadOnlyArray<T> array && this == array;
        }

        public bool Equals(ReadOnlyArray<T> other)
        {
            return other == this;
        }

        public override int GetHashCode()
        {
            return m_list.GetHashCode();
        }

        // -- //

        public static bool operator ==(ReadOnlyArray<T> a, ReadOnlyArray<T> b)
        {
            return a.m_list == b.m_list;
        }

        public static bool operator !=(ReadOnlyArray<T> a, ReadOnlyArray<T> b)
        {
            return a.m_list != b.m_list;
        }
    }
}
