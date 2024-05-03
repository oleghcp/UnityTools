using System;
using UnityEngine;

namespace OlegHcp.NodeBased.Service
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
        internal abstract NodeType NodeType { get; }

#if UNITY_EDITOR
        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string NameFieldName => nameof(NodeName);
        internal static string GraphFieldName => nameof(Owner);
        internal static string ArrayFieldName => nameof(Next);
#endif
    }
}
