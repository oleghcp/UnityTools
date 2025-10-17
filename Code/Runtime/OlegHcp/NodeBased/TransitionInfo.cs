using OlegHcp.NodeBased.Service;

namespace OlegHcp.NodeBased
{
    public struct TransitionInfo<TNode> where TNode : Node<TNode>
    {
        private Condition _condition;
        private RawNode _to;

        public Condition Condition => _condition;
        public TNode NextNode => _to as TNode;
        public bool Exists => _to is ExitNode;

        internal TransitionInfo(Condition condition, RawNode to)
        {
            _condition = condition;
            _to = to;
        }

        public bool Available(object data = null)
        {
            return _condition == null || _condition.Check(data);
        }
    }
}
