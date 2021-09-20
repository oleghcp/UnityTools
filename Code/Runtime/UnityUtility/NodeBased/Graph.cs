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
        [SerializeReference]
        private protected RawNode[] _nodes;

        private Dictionary<int, RawNode> _dict;

        private protected Dictionary<int, RawNode> Dict
        {
            get
            {
                if (_dict == null)
                    _dict = _nodes.ToDictionary(key => key.Id, value => value);

                return _dict;
            }
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
        internal static string NodesFieldName => nameof(_nodes);
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

        internal IEnumerator<Transition<TNode>> GetEnumeratorFor(RawNode node)
        {
            Transition[] next = node.Next;

            for (int i = 0; i < next.Length; i++)
            {
                RawNode nextNode = Dict[next[i].NextNodeId];

                if (nextNode is HubNode)
                {
                    var enumerator = GetEnumeratorFor(nextNode);

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
                else
                {
                    yield return new Transition<TNode>(next[i].Condition, node, nextNode);
                }
            }
        }

        public StateMachine<TState, TData> CreateStateMachine<TState, TData>(TData data,
                                                                             Action<TState, TState> onStateChanging = null,
                                                                             Action finalCallback = null)
            where TState : class, IState where TData : class
        {
            var stateMachine = new StateMachine<TState, TData>(data, onStateChanging, finalCallback);
            var states = new Dictionary<RawNode, TState>(_nodes.Length);

            RawNode rootNode = RootNode;

            for (int i = 0; i < _nodes.Length; i++)
            {
                RawNode node = _nodes[i];

                if (node is HubNode)
                    continue;

                TState state = states.Place(node, node.CreateState<TState>());

                if (state != null)
                    stateMachine.AddState(state, node == rootNode);
            }

            for (int i = 0; i < _nodes.Length; i++)
            {
                RawNode node = _nodes[i];

                if (node.ServiceNode())
                    continue;

                var enumerator = GetEnumeratorFor(node);

                while (enumerator.MoveNext())
                {
                    Transition<TNode> transition = enumerator.Current;

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
