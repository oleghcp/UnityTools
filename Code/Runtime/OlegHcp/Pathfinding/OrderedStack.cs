using System.Collections.Generic;
using OlegHcp.Pool;
using OlegHcp.Pool.Storages;
using OlegHcp.Tools;

namespace OlegHcp.Pathfinding
{
    internal class OrderedStack : HashSet<IPathNode>
    {
        private static ObjectPool<ValueNode> _objectPool = new(new ConcurrentQueueStorage<ValueNode>(),
                                                               () => new ValueNode());
        private ValueNode _first;
        private ValueNode _last;

        public void Push(IPathNode value, float priority)
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
                if (n.Priority < priority)
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

        public IPathNode Pop()
        {
            if (Count == 0)
                throw ThrowErrors.NoElements();

            ValueNode lastNode = _last;
            IPathNode element = lastNode.Element;
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
            public IPathNode Element { get; private set; }
            public float Priority { get; private set; }
            public ValueNode Prev { get; set; }
            public ValueNode Next { get; set; }

            public void SetUp(IPathNode value, float priority)
            {
                Element = value;
                Priority = priority;
            }

            #region IPoolable
            void IPoolable.Reinit()
            {
            }

            void IPoolable.CleanUp()
            {
                Element = default;
                Next = null;
                Prev = null;
            }
            #endregion
        }
    }
}
