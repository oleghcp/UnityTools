using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#pragma warning disable CS0659, CS0661, IDE0016
namespace UnityUtility.Collections
{
    public struct ReadOnlyArray<T> : IEnumerable<T>, IEquatable<ReadOnlyArray<T>>, IList<T>, IList
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

        public T GetLast()
        {
            return m_list[m_list.Count - 1];
        }

        public ReadOnlyCollection<T> ToCollection()
        {
            return new ReadOnlyCollection<T>(m_list);
        }

        public T[] ToArray()
        {
            return m_list.ToArray();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_list.CopyTo(array, arrayIndex);
        }

        // -- //

        #region IList
        object IList.this[int index]
        {
            get { return m_list[index]; }
            set { throw new NotImplementedException(); }
        }

        T IList<T>.this[int index]
        {
            get { return m_list[index]; }
            set { throw new NotImplementedException(); }
        }

        bool IList.IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
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

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            return value is T && m_list.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            if (value is T)
                return m_list.IndexOf((T)value);

            return -1;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ICollection collection = m_list as ICollection;

            if (m_list != null)
            {
                collection.CopyTo(array, index);
            }
            else
            {
                for (int i = index; i < m_list.Count; i++)
                {
                    array.SetValue(m_list[i], i);
                }
            }
        }
        #endregion

        // -- //

        public override bool Equals(object obj)
        {
            return obj is ReadOnlyArray<T> && this == (ReadOnlyArray<T>)obj;
        }

        public bool Equals(ReadOnlyArray<T> other)
        {
            return other == this;
        }

        public override int GetHashCode()
        {
            return m_list.GetHashCode();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
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
