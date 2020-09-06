using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

#if UNITY_2020_1_OR_NEWER
namespace UnityUtility.Collections
{
    [Serializable]
    public sealed class ReorderableArray<T> : DrawnArray<T>
    {
        [SerializeField]
        private T[] m_array;

        protected override T[] Items
        {
            get { return m_array; }
        }

        [Preserve]
        private ReorderableArray() { }

        public ReorderableArray(int length)
        {
            m_array = new T[length];
        }
    }

    [Serializable]
    public sealed class ReorderableRefArray<T> : DrawnArray<T> where T : class
    {
        [SerializeReference, SerializeReferenceSelection]
        private T[] m_array;

        protected override T[] Items
        {
            get { return m_array; }
        }

        [Preserve]
        private ReorderableRefArray() { }

        public ReorderableRefArray(int length)
        {
            m_array = new T[length];
        }
    }

    public abstract class DrawnArray<T> : IList<T>, IReadOnlyList<T>
    {
        protected abstract T[] Items { get; }

        public int Count
        {
            get { return Items.Length; }
        }

        public ref T this[int index]
        {
            get { return ref Items[index]; }
        }

        T IReadOnlyList<T>.this[int index]
        {
            get { return Items[index]; }
        }

        T IList<T>.this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            return (Items as IList<T>).IndexOf(item);
        }

        public bool Contains(T item)
        {
            return (Items as IList<T>).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
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
            return (Items as IList<T>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
#endif
