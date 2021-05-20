using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility.NodeBased
{
    public abstract class RawGraph : ScriptableObject
    {
        [SerializeField]
        internal RawNode[] Nodes;

#if UNITY_EDITOR
        [SerializeField]
        internal int LastId;
        [SerializeField]
        private float _nodeWidth;

        internal static string IdGeneratorFieldName => nameof(LastId);
        internal static string WidthFieldName => nameof(_nodeWidth);
        internal static string ArrayFieldName => nameof(Nodes);
#endif
    }

    public abstract class Graph<TNode, TTransition> : RawGraph where TNode : Node<TTransition> where TTransition : Transition, new()
    {
        public TNode RootNode => Nodes.Length > 0 ? (TNode)Nodes[0] : null;

        public TNode GetNodeById(int id)
        {
            return Nodes.Find(item => ((TNode)item).LocalId == id) as TNode;
        }

#if UNITY_EDITOR
        internal Type GetNodeType() => typeof(TNode);
        internal Type GetTransitionType() => typeof(TTransition);

        internal static string GetNodeTypeMethodName => nameof(GetNodeType);
        internal static string GetTransitionTypeMethodName => nameof(GetTransitionType);
#endif
    }

    public abstract class Graph<TNode> : Graph<TNode, Transition> where TNode : Node<Transition>
    {

    }
}
