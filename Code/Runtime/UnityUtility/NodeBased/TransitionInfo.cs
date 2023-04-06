using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
{
    public struct TransitionInfo<TNode> where TNode : Node<TNode>
    {
        private Condition _condition;
        private RawNode _from;
        private RawNode _to;

        internal Condition Condition => _condition;
        public TNode NextNode => _to as TNode;
        public bool IsExit => _to is ExitNode;

        internal TransitionInfo(Condition condition, RawNode from, RawNode to)
        {
            _condition = condition;
            _from = from;
            _to = to;
        }

        public bool Available(object data = null)
        {
            return _condition == null || _condition.Check(_from, data);
        }
    }
}
