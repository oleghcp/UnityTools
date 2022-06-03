#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    internal enum NodeType : byte
    {
        Real,
        Hub,
        Exit,
        Common,
    }

    [Serializable]
    public abstract class RawNode
    {
        [SerializeField]
        internal Vector2 Position;
        [SerializeField]
        internal string NodeName;
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal RawGraph Owner;
        [SerializeField]
        internal Transition[] Next;

        public int LocalId => Id;
        public string Name => NodeName;
        internal virtual NodeType NodeType => NodeType.Real;

        public virtual TState CreateState<TState>() where TState : class, IState
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR
        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string NameFieldName => nameof(NodeName);
        internal static string GraphFieldName => nameof(Owner);
        internal static string ArrayFieldName => nameof(Next);

        internal static NodeType GetNodeType(Type type)
        {
            if (type.Is(typeof(ExitNode)))
                return NodeType.Exit;
            else if (type.Is(typeof(HubNode)))
                return NodeType.Hub;
            else if (type.Is(typeof(CommonNode)))
                return NodeType.Common;
            else
                return NodeType.Real;
        }
#endif
    }

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
#endif
