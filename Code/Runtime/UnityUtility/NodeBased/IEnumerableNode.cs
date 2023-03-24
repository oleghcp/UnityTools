using System.Collections.Generic;

namespace UnityUtility.NodeBased
{
    public interface IEnumerableNode<TNode> : IEnumerable<Transition<TNode>> where TNode : Node<TNode>
    {
        new NodeEnumerator<TNode> GetEnumerator();
    }
}
