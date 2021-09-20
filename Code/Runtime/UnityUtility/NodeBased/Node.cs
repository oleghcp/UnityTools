using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    public enum NodeType : byte
    {
        Real,
        Hub,
        Exit,
    }

    [Serializable]
    public abstract class RawNode
    {
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
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string NameFieldName => nameof(NodeName);
        internal static string GraphFieldName => nameof(Owner);
        internal static string ArrayFieldName => nameof(Next);
#endif
    }

    [Serializable]
    public abstract class Node<TNode> : RawNode, IEnumerable<Transition<TNode>> where TNode : Node<TNode>
    {
        public Graph<TNode> Graph => Owner as Graph<TNode>;

        public IEnumerator<Transition<TNode>> GetEnumerator()
        {
            return Graph.GetEnumeratorFor(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
