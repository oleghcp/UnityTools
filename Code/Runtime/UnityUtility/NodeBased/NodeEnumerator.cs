using System.Collections;
using System.Collections.Generic;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
{
    public struct NodeEnumerator<TNode> : IEnumerator<Transition<TNode>> where TNode : Node<TNode>
    {
        private RawNode _node;
        private HubNode _hub;
        private int _index;
        private int _subIndex;

        public Transition<TNode> Current
        {
            get
            {
                if (_node == null)
                    return default;

                int index = (_hub != null ? _subIndex : _index) - 1;
                Transition[] array = (_hub ?? _node).Next;

                if ((uint)index >= (uint)array.Length)
                    return default;

                RawNode nextNode = _node.Owner.Dict[array[index].NextNodeId];
                return new Transition<TNode>(array[index].Condition, _node, nextNode);
            }
        }

        object IEnumerator.Current => Current;

        internal NodeEnumerator(RawNode node)
        {
            _node = node;
            _index = 0;
            _hub = null;
            _subIndex = 0;
        }

        public bool MoveNext()
        {
            if (_node == null)
                return false;

            if (MoveNext(_hub, ref _subIndex))
                return true;

            if (MoveNext(_node, ref _index))
                return true;

            _index = int.MaxValue;
            _subIndex = int.MaxValue;
            _hub = null;
            return false;
        }

        public void Reset()
        {
            _index = 0;
            _hub = null;
            _subIndex = 0;
        }

        public void Dispose() { }

        private bool MoveNext(RawNode node, ref int index)
        {
            if (node == null)
                return false;

            Transition[] array = node.Next;

            if ((uint)index >= (uint)array.Length)
                return false;

            Transition transition = array[index];
            RawNode nextNode = node.Owner.Dict[transition.NextNodeId];
            index++;

            if (!(nextNode is HubNode hub))
                return true;

            _hub = hub;
            _subIndex = 0;

            if (MoveNext(hub, ref _subIndex))
                return true;

            _hub = null;

            return MoveNext(node, ref index);
        }
    }
}
