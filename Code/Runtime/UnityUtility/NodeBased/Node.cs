using UnityEngine;

namespace UnityUtility.NodeBased
{
    public abstract class RawNode : ScriptableObject
    {
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal RawGraph Owner;

        public int LocalId => Id;
        public RawGraph Graph => Owner;

#if UNITY_EDITOR
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string GraphFieldName => nameof(Owner);
#endif
    }

    public abstract class Node<TTransition> : RawNode where TTransition : Transition, new()
    {
        [SerializeField]
        internal TTransition[] Next;

#if UNITY_EDITOR
        internal static string ArrayFieldName => nameof(Next);
#endif
    }

    public abstract class Node : Node<Transition> { }
}
