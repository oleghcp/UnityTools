using System.Collections;
using System.Collections.Generic;

namespace OlegHcp.NodeBased.Service
{
    public struct NodeEnumerator<TNode> : IEnumerator<TransitionInfo<TNode>> where TNode : Node<TNode>
    {
        private RawNode _node;
        private HubNode _hub;
        private int _index;
        private int _subIndex;

        public TransitionInfo<TNode> Current
        {
            get
            {
                if (_node == null)
                    return default;

                bool isHub = _hub != null;
                Transition[] array = (isHub ? _hub : _node).Next;
                int index = (isHub ? _subIndex : _index) - 1;

                if ((uint)index >= (uint)array.Length)
                    return default;

                Transition transition = array[index];
                RawNode nextNode = _node.Owner.Dict[transition.NextNodeId];
                return new TransitionInfo<TNode>(transition.Condition, nextNode);
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
