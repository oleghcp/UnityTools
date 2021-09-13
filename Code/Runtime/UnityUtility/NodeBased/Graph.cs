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

        public RawNode RootNode => Nodes.Length > 0 ? Nodes[0] : null;

        public RawNode GetNodeById(int id)
        {
            return Nodes.Find(item => item.LocalId == id);
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
                var (valid, state) = node.CreateState<TState>();

                if (valid)
                {
                    states[node] = state;
                    stateMachine.AddState(state, node == rootNode);
                }
                else
                {
                    states[node] = null;
                }
            }

            foreach (RawNode node in Nodes)
            {
                foreach (Transition transition in node.Transitions)
                {
                    stateMachine.AddTransition(states[node],
                                               transition.CreateCondition<TState, TData>(),
                                               states[transition.NextNode]);
                }
            }

            return stateMachine;
        }

#if UNITY_EDITOR
        [SerializeField]
        internal int LastId;
        [SerializeField]
        private float _nodeWidth;

        internal abstract Type GetNodeType();
        internal static string IdGeneratorFieldName => nameof(LastId);
        internal static string WidthFieldName => nameof(_nodeWidth);
        internal static string ArrayFieldName => nameof(Nodes);
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

#if UNITY_EDITOR
        internal sealed override Type GetNodeType() => typeof(TNode);
#endif
    }
}
