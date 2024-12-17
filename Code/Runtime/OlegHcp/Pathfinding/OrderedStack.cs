using System;
using System.Collections.Generic;
using OlegHcp.Pool;
using OlegHcp.Pool.Storages;
using OlegHcp.Tools;

namespace OlegHcp.Pathfinding
{
    internal class OrderedStack<TValue, TPriority> : HashSet<TValue> where TPriority : IComparable<TPriority>
    {
        private ObjectPool<ValueNode> _objectPool = new ObjectPool<ValueNode>(new StackStorage<ValueNode>(), () => new ValueNode());

        private ValueNode _first;
        private ValueNode _last;

        public void Push(TValue value, TPriority priority)
        {
            if (!Add(value))
                return;

            ValueNode newNode = _objectPool.Get();
            newNode.SetUp(value, priority);

            if (Count == 1)
            {
                _first = newNode;
                _last = newNode;
                return;
            }

            ValueNode nextNode = null;
            ValueNode prevNode;

            for (ValueNode n = _first; n != null; n = n.Next)
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

            ValueNode lastNode = _last;
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

            for (ValueNode n = _first?.Next; n != null; n = n.Next)
            {
                _objectPool.Release(n.Prev);
            }

            _objectPool.Release(_last);

            _first = null;
            _last = null;
            base.Clear();
        }

        private class ValueNode : IPoolable
        {
            public TValue Element;
            public TPriority Priority;
            public ValueNode Prev;
            public ValueNode Next;

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
