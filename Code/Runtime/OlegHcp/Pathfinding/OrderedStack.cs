using System;
using System.Collections.Generic;
using OlegHcp.Pool;
using OlegHcp.Pool.Storages;
using OlegHcp.Tools;

namespace OlegHcp.Pathfinding
{
    internal class OrderedStack<TValue, TPriority> : HashSet<TValue> where TPriority : IComparable<TPriority>
    {
        private ObjectPool<Node> _objectPool = new ObjectPool<Node>(new StackStorage<Node>(), () => new Node());

        private Node _first;
        private Node _last;

        public void Push(TValue value, TPriority priority)
        {
            if (!Add(value))
                return;

            Node newNode = _objectPool.Get();
            newNode.SetUp(value, priority);

            if (Count == 1)
            {
                _first = newNode;
                _last = newNode;
                return;
            }

            Node nextNode = null;
            Node prevNode;

            for (Node n = _first; n != null; n = n.Next)
            {
                if (n.Priority.CompareTo(priority) < 0)
                {
                    nextNode = n;
                    break;
                }
            }

            if (nextNode == null)
            {
                prevNode = _last;
                newNode.Prev = prevNode;
                prevNode.Next = newNode;
                _last = newNode;
                return;
            }

            prevNode = nextNode.Prev;
            newNode.Next = nextNode;
            nextNode.Prev = newNode;

            if (prevNode == null)
            {
                _first = newNode;
            }
            else
            {
                prevNode.Next = newNode;
                newNode.Prev = prevNode;
            }
        }

        public TValue Pop()
        {
            if (Count == 0)
                throw ThrowErrors.NoElements();

            Node lastNode = _last;
            TValue element = lastNode.Element;
            Remove(element);

            if (Count == 0)
            {
                _first = null;
                _last = null;
            }
            else
            {
                _last = lastNode.Prev;
                _last.Next = null;
            }

            _objectPool.Release(lastNode);

            return element;
        }

        public new void Clear()
        {
            if (Count == 0)
                return;

            for (Node n = _first?.Next; n != null; n = n.Next)
            {
                _objectPool.Release(n.Prev);
            }

            _objectPool.Release(_last);

            _first = null;
            _last = null;
            base.Clear();
        }

        private class Node : IPoolable
        {
            public TValue Element;
            public TPriority Priority;
            public Node Prev;
            public Node Next;

            public void SetUp(TValue value, TPriority priority)
            {
                Element = value;
                Priority = priority;
            }

            #region IPoolable
            public void Reinit() { }

            public void CleanUp()
            {
                Element = default;
                Next = null;
                Prev = null;
            }
            #endregion
        }
    }
}
