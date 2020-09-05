using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

#if UNITY_2020_1_OR_NEWER
namespace UnityUtility.Collections
{
    [Serializable]
    public class DrawnArray<T> : IList<T>, IReadOnlyList<T>
    {
        [SerializeField]
        private T[] m_array;

        public int Count
        {
            get { return m_array.Length; }
        }

        public ref T this[int index]
        {
            get { return ref m_array[index]; }
        }

        T IReadOnlyList<T>.this[int index]
        {
            get { return m_array[index]; }
        }

        T IList<T>.this[int index]
        {
            get { return m_array[index]; }
            set { m_array[index] = value; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        [Preserve]
        private DrawnArray() { }

        public DrawnArray(int length)
        {
            m_array = new T[length];
        }

        public int IndexOf(T item)
        {
            return m_array.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return m_array.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_array.CopyTo(array, arrayIndex);
        }

        #region Not Supported
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item)
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
        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            return (m_array as IList<T>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_array.GetEnumerator();
        }
    }
}
#endif
