using System;
using System.Collections.Generic;
using OlegHcp.Pool;
using OlegHcp.Tools;

namespace OlegHcp.Pathfinding
{
#if UNITY
    [Serializable]
#endif
    internal class OrderedStack<TValue, TPriority> : HashSet<TValue> where TPriority : IComparable<TPriority>
    {
        private ObjectPool<ListNode> _objectPool = new ObjectPool<ListNode>(() => new ListNode());

        private ListNode _first;
        private ListNode _last;

        public void Push(TValue value, TPriority priority)
        {
            if (!Add(value))
                return;

            ListNode newNode = _objectPool.Get().SetUp(value, priority);

            if (Count == 1)
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
            if (Count == 0)
                throw ThrowErrors.NoElements();

            ListNode lastNode = _last;
            TValue element = lastNode.Element;
            Remove(element);

            if (Count == 0)
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

        public new void Clear()
        {
            if (Count == 0)
                return;

            for (ListNode n = _first?.Right; n != null; n = n.Right)
            {
                _objectPool.Release(n.Left);
            }

            _objectPool.Release(_last);

            _first = null;
            _last = null;
            base.Clear();
        }

#if UNITY
        [Serializable]
#endif
        private class ListNode : IPoolable
        {
            public TValue Element;
            public TPriority Priority;
            public ListNode Left;
            public ListNode Right;

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
