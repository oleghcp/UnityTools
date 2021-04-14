using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.NodeBased
{
    public abstract class Graph : ScriptableObject
    {
        [SerializeField]
        internal Node[] Nodes;

        public Node RootNode => Nodes.Length > 0 ? Nodes[0] : null;

#if UNITY_EDITOR
        [SerializeField]
        internal int LastId;
        [SerializeField]
        internal Vector2 CameraPosition;
        [SerializeField]
        private float _nodeWidth;

        internal static string IdGeneratorFieldName => nameof(LastId);
        internal static string WidthFieldName => nameof(_nodeWidth);
        internal static string CameraPositionFieldName => nameof(CameraPosition);
        internal static string ArrayFieldName => nameof(Nodes);
#endif
    }

    public abstract class Graph<TNode, TTransition> : Graph where TNode : Node where TTransition : Transition, new()
    {
        public new TNode RootNode => base.RootNode as TNode;

        public TNode GetNodeById(int id)
        {
            return Nodes.Find(item => item.LocalId == id) as TNode;
        }

        public Connection<TNode, TTransition> GetTransitons(TNode node)
        {
            return new Connection<TNode, TTransition>(node);
        }

#if UNITY_EDITOR
        internal Type GetNodeType() => typeof(TNode);
        internal Type GetTransitionType() => typeof(TTransition);

        internal static string GetNodeTypeMethodName => nameof(GetNodeType);
        internal static string GetTransitionTypeMethodName => nameof(GetTransitionType);
#endif
    }

    public abstract class Graph<TNode> : Graph<TNode, Transition> where TNode : Node
    {

    }
}
#endif
