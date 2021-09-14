using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    public abstract class RawGraph : ScriptableObject
    {
        [SerializeField]
        internal RawNode[] Nodes;

        public RawNode RootNode
        {
            get
            {
                if (Nodes.Length == 0)
                    return null;

                return Nodes.Find(item => item.RealNode());
            }
        }

        public RawNode GetNodeById(int id)
        {
            return Nodes.Find(item => item.RealNode() && item.LocalId == id);
        }

#if UNITY_EDITOR
        [SerializeField]
        internal int LastId;
        [SerializeField]
        private float _nodeWidth;

        internal abstract Type GetNodeType();
        internal static string IdGeneratorFieldName => nameof(LastId);
        internal static string WidthFieldName => nameof(_nodeWidth);
        internal static string NodesFieldName => nameof(Nodes);
#endif
    }

    public abstract class Graph<TNode> : RawGraph where TNode : Node<TNode>
    {
        public new TNode RootNode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (TNode)base.RootNode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new TNode GetNodeById(int id)
        {
            return base.GetNodeById(id) as TNode;
        }

        public StateMachine<TState, TData> CreateStateMachine<TState, TData>(TData data,
                                                                             Action<TState, TState> onStateChanging = null,
                                                                             Action finalCallback = null)
            where TState : class, IState where TData : class
        {
            var stateMachine = new StateMachine<TState, TData>(data, onStateChanging, finalCallback);
            var states = new Dictionary<RawNode, TState>(Nodes.Length);

            RawNode rootNode = RootNode;

            foreach (RawNode node in Nodes)
            {
                if (node.ServiceNode())
                    continue;

                TState state = states.Place(node, node.CreateState<TState>());

                if (state != null)
                    stateMachine.AddState(state, node == rootNode);
            }

            foreach (RawNode node in Nodes)
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

            return stateMachine;
        }

#if UNITY_EDITOR
        internal sealed override Type GetNodeType() => typeof(TNode);
#endif
    }
}
