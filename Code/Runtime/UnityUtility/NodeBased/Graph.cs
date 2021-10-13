#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    public abstract class RawGraph : ScriptableObject
    {
        [SerializeReference]
        private RawNode[] _nodes;

        private Dictionary<int, RawNode> _dict;

        internal Dictionary<int, RawNode> Dict
        {
            get => _dict ?? (_dict = _nodes.ToDictionary(key => key.Id, value => value));
        }

        public RawNode GetNodeById(int id)
        {
            if (Dict.TryGetValue(id, out RawNode value) && value.RealNode())
                return value;

            return null;
        }

        public RawNode RootNode
        {
            get
            {
                if (_nodes.Length == 0)
                    return null;

                return _nodes.Find(item => item.RealNode());
            }
        }

        internal abstract void InitializeMachine<TState, TData>(StateMachine<TState, TData> stateMachine) where TState : class, IState;


#if UNITY_EDITOR
        [SerializeField]
        internal int LastId;
        [SerializeField]
        private float _nodeWidth;

        internal abstract Type GetNodeType();
        internal static string IdGeneratorFieldName => nameof(LastId);
        internal static string WidthFieldName => nameof(_nodeWidth);
        internal static string NodesFieldName => nameof(_nodes);
#endif
    }

    public abstract class Graph<TNode> : RawGraph where TNode : Node<TNode>
    {
        private ReadOnlyCollection<TNode> _nodeList;

        public new TNode RootNode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (TNode)base.RootNode;
        }

        public ReadOnlyCollection<TNode> Nodes
        {
            get
            {
                if (_nodeList == null)
                {
                    TNode[] nodes = Dict.Values.Where(item => item.RealNode())
                                               .Select(item => (TNode)item)
                                               .ToArray();
                    _nodeList = new ReadOnlyCollection<TNode>(nodes);
                }

                return _nodeList;
            }
        }

        public new TNode GetNodeById(int id)
        {
            return base.GetNodeById(id) as TNode;
        }

        internal override void InitializeMachine<TState, TData>(StateMachine<TState, TData> stateMachine)
        {
            Dictionary<RawNode, TState> states = new Dictionary<RawNode, TState>(Dict.Count);

            RawNode rootNode = RootNode;

            foreach (RawNode node in Dict.Values)
            {
                if (node is HubNode)
                    continue;

                TState state = states.Place(node, node.CreateState<TState>());

                if (state != null)
                    stateMachine.AddState(state, node == rootNode);
            }

            foreach (RawNode node in Dict.Values)
            {
                if (node.ServiceNode())
                    continue;

                foreach (Transition<TNode> transition in node as TNode)
                {
                    stateMachine.AddTransition(states[node],
                                               transition.CreateCondition<TState, TData>(),
                                               states[transition.NextNode]);
                }
            }
        }

#if UNITY_EDITOR
        internal sealed override Type GetNodeType() => typeof(TNode);
#endif
    }
}
#endif
