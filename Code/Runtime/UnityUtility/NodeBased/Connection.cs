using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityUtility.NodeBased
{
    public struct Connection<TNode, TTransition> where TNode : Node where TTransition : Transition, new()
    {
        private TNode _node;

        public int Count => _node.Next.Length;
        public Pair this[int index] => Get(index);

        internal Connection(TNode node)
        {
            _node = node;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerable<Pair> AsEnumerable()
        {
            for (int i = 0; i < _node.Next.Length; i++)
            {
                yield return Get(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pair Get(int index)
        {
            return new Pair((TNode)_node.Next[index].Node, (TTransition)_node.Next[index]);
        }

        public struct Pair
        {
            public TNode NextNode { get; }
            public TTransition Transition { get; }

            internal Pair(TNode node, TTransition transition)
            {
                NextNode = node;
                Transition = transition;
            }
        }

        public struct Enumerator : IEnumerator<Pair>
        {
            private readonly Connection<TNode, TTransition> _info;
            private int _currentIndex;

            public Pair Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _info[_currentIndex];
            }

            object IEnumerator.Current => Current;

            internal Enumerator(Connection<TNode, TTransition> info)
            {
                _info = info;
                _currentIndex = -1;
            }

            public bool MoveNext()
            {
                int num = _currentIndex + 1;
                if (num < _info.Count)
                {
                    _currentIndex = num;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _currentIndex = 0;
            }

            public void Dispose() { }
        }
    }
}
