#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;

namespace UnityUtility.NodeBased
{
    public interface IEnumerableNode<TNode> : IEnumerable<Transition<TNode>> where TNode : Node<TNode>
    {
        new NodeEnumerator<TNode> GetEnumerator();
    }
}
#endif
