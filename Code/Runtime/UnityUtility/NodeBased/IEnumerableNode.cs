using System.Collections.Generic;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
{
    public interface IEnumerableNode<TNode> : IEnumerable<TransitionInfo<TNode>> where TNode : Node<TNode>
    {
        new NodeEnumerator<TNode> GetEnumerator();
    }
}
