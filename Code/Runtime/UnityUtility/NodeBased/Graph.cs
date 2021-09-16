using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    public abstract class RawGraph : ScriptableObject
    {
        [SerializeField]
        internal RawNode[] Nodes;
        private Dictionary<int, RawNode> _dict;

        protected Dictionary<int, RawNode> Dict
        {
            get
            {
                if (_dict == null)
                    _dict = Nodes.ToDictionary(key => key.Id, value => value);

                return _dict;
            }
        }

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
            if (Dict.TryGetValue(id, out RawNode value) && value.RealNode())
                return value;

            return null;
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

        public IEnumerable<Transition<TNode>> GetNextFor(RawNode node)
        {
            foreach (Transition transition in node.Next)
            {
                RawNode nextNode = Dict[transition.NextNodeId];

                if (nextNode is HubNode)
                {
                    foreach (Transition<TNode> item in GetNextFor(nextNode))
                        yield return item;
                }
                else
                {
                    yield return new Transition<TNode>(transition, nextNode);
                }
            }
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
                if (node is HubNode)
                    continue;

                TState state = states.Place(node, node.CreateState<TState>());

                if (state != null)
                    stateMachine.AddState(state, node == rootNode);
            }

            foreach (RawNode node in Nodes)
            {
                if (node.ServiceNode())
                    continue;

                foreach (Transition<TNode> transition in GetNextFor(node))
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
