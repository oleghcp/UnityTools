using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.Collections;
using UnityEngine;

namespace OlegHcp.NodeBased.Service
{
    public abstract class RawGraph : ScriptableObject
    {
        [SerializeField]
        internal int LastId;
        [SerializeField]
        private float _nodeWidth;
        [SerializeReference]
        private RawNode[] _nodes;
        [SerializeField]
        private int _rootNodeId;
        [SerializeReference]
        private RawNode _commonNode;

        private Dictionary<int, RawNode> _dict;

        internal RawNode RootNode => GetNodeById(_rootNodeId);
        internal RawNode CommonNode => _commonNode;
        internal abstract Type RootNodeType { get; }
        protected RawNode[] NodeArray => _nodes;

        internal Dictionary<int, RawNode> Dict => _dict ?? (_dict = _nodes.ToDictionary(key => key.Id, value => value));

        public virtual Type GetConditionRootType()
        {
            return typeof(Condition);
        }

        public abstract Type GetNodeRootType();

        internal RawNode GetNodeById(int id)
        {
            if (Dict.TryGetValue(id, out RawNode value) && value.IsRegular())
                return value;

            return null;
        }

        internal abstract void InitializeStateMachine<TState, TData>(StateMachine<TState, TData> stateMachine) where TState : class, IState;

#if UNITY_EDITOR
        internal static string IdGeneratorFieldName => nameof(LastId);
        internal static string WidthFieldName => nameof(_nodeWidth);
        internal static string NodesFieldName => nameof(_nodes);
        internal static string RootNodeFieldName => nameof(_rootNodeId);
        internal static string CommonNodeFieldName => nameof(_commonNode);
#endif
    }
}
