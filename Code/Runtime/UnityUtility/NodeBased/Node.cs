using System;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    public abstract class RawNode : ScriptableObject
    {
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal RawGraph Owner;
        [SerializeField]
        internal Transition[] Next;

        public int LocalId => Id;

        public virtual TState CreateState<TState>() where TState : class, IState
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string GraphFieldName => nameof(Owner);
        internal static string ArrayFieldName => nameof(Next);
#endif
    }

    public abstract class Node<TNode> : RawNode where TNode : Node<TNode>
    {
        public Graph<TNode> Graph => Owner as Graph<TNode>;
    }
}
