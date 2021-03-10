using UnityEngine;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.NodeBased
{
    public abstract class Node : ScriptableObject
    {
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal Graph Owner;
        [SerializeReference]
        internal Transition[] Next;

        public int LocalId => Id;
        public Graph Graph => Owner;

#if UNITY_EDITOR
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string ArrayFieldName => nameof(Next);
        internal static string GraphFieldName => nameof(Owner);
#endif
    }
}
#endif
