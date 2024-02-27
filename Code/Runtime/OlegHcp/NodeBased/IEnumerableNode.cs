using System.Collections.Generic;
using OlegHcp.NodeBased.Service;

namespace OlegHcp.NodeBased
{
    public interface IEnumerableNode<TNode> : IEnumerable<TransitionInfo<TNode>> where TNode : Node<TNode>
    {
        new NodeEnumerator<TNode> GetEnumerator();
    }
}
