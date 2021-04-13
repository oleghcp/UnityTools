using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityUtility.NodeBased
{
    public struct TransitionsInfo<TNode, TTransition> : IEnumerable<TTransition> where TNode : Node where TTransition : Transition<TNode>, new()
    {
        private TNode _node;

        public int Count => _node.Next.Length;

        public TTransition this[int index] => _node.Next[index] as TTransition;

        internal TransitionsInfo(TNode node)
        {
            _node = node;
        }

        IEnumerator<TTransition> IEnumerable<TTransition>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<TTransition>
        {
            private readonly TransitionsInfo<TNode, TTransition> _info;
            private int _index;

            public TTransition Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _info[_index];
            }

            object IEnumerator.Current => Current;

            internal Enumerator(TransitionsInfo<TNode, TTransition> info)
            {
                _info = info;
                _index = -1;
            }

            public bool MoveNext()
            {
                int num = _index + 1;
                if (num < _info.Count)
                {
                    _index = num;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _index = 0;
            }

            public void Dispose() { }
        }
    }
}
