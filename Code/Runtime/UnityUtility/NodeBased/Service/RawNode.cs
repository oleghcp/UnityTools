using System;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased.Service
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
            if (type == typeof(ExitNode))
                return NodeType.Exit;
            else if (type == typeof(HubNode))
                return NodeType.Hub;
            else if (type == typeof(CommonNode))
                return NodeType.Common;
            else
                return NodeType.Real;
        }
#endif
    }
}
