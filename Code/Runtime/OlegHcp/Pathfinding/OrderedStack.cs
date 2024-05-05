using System;
using System.Collections.Generic;
using OlegHcp.Pool;
using OlegHcp.Tools;

namespace OlegHcp.Pathfinding
{
    [Serializable]
    internal class OrderedStack<TValue, TPriority> where TValue : IEquatable<TValue> where TPriority : IComparable<TPriority>
    {
        private HashSet<TValue> _values = new HashSet<TValue>();
        private ObjectPool<ListNode> _objectPool = new ObjectPool<ListNode>(() => new ListNode());

        private ListNode _first;
        private ListNode _last;

        public int Count => _values.Count;

        public void Push(TValue value, TPriority priority)
        {
            if (!_values.Add(value))
                return;

            ListNode newNode = _objectPool.Get().SetUp(value, priority);

            if (_values.Count == 1)
            {
                _first = newNode;
                _last = newNode;
                return;
            }

            ListNode rightNode = null;
            ListNode leftNode;

            for (ListNode n = _first; n != null; n = n.Right)
            {
                if (n.Priority.CompareTo(priority) < 0)
                {
                    rightNode = n;
                    break;
                }
            }

            if (rightNode == null)
            {
                leftNode = _last;
                newNode.Left = leftNode;
                leftNode.Right = newNode;
                _last = newNode;
                return;
            }

            leftNode = rightNode.Left;
            newNode.Right = rightNode;
            rightNode.Left = newNode;

            if (leftNode == null)
            {
                _first = newNode;
            }
            else
            {
                leftNode.Right = newNode;
                newNode.Left = leftNode;
            }
        }

        public TValue Pop()
        {
            if (_values.Count == 0)
                throw ThrowErrors.NoElements();

            ListNode lastNode = _last;
            TValue element = lastNode.Element;
            _values.Remove(element);

            if (_values.Count == 0)
            {
                _first = null;
                _last = null;
            }
            else
            {
                _last = lastNode.Left;
                _last.Right = null;
            }

            _objectPool.Release(lastNode);

            return element;
        }

        public bool Contains(TValue element)
        {
            return _values.Contains(element);
        }

        public void Clear()
        {
            if (_values.Count == 0)
                return;

            for (ListNode n = _first?.Right; n != null; n = n.Right)
            {
                if (n.Left != null)
                    _objectPool.Release(n.Left);
            }

            _objectPool.Release(_last);

            _first = null;
            _last = null;
            _values.Clear();
        }

        [Serializable]
        private class ListNode : IPoolable
        {
            public TValue Element;
            public TPriority Priority;
            public ListNode Right;
            public ListNode Left;

            public ListNode SetUp(TValue value, TPriority priority)
            {
                Element = value;
                Priority = priority;
                return this;
            }

            public void Deconstruct(out TValue element, out TPriority priority)
            {
                element = Element;
                priority = Priority;
            }

            #region IPoolable
            public void Reinit() { }

            public void CleanUp()
            {
                Element = default;
                Right = null;
                Left = null;
            }
            #endregion
        }
    }
}
