using System;
using System.Collections;
using System.Collections.Generic;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
{
    [Serializable]
    public abstract class Node<TNode> : RawNode, IEnumerableNode<TNode> where TNode : Node<TNode>
    {
        public Graph<TNode> Graph => Owner as Graph<TNode>;

        public NodeEnumerator<TNode> GetEnumerator()
        {
            return new NodeEnumerator<TNode>(this as TNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NodeEnumerator<TNode>(this as TNode);
        }

        IEnumerator<Transition<TNode>> IEnumerable<Transition<TNode>>.GetEnumerator()
        {
            return new NodeEnumerator<TNode>(this as TNode);
        }
    }
}
