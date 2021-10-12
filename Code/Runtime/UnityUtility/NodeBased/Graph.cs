#if UNITY_2019_3_OR_NEWER
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

    public abstract class Graph<TNode> : RawGraph, ISerializationCallbackReceiver where TNode : Node<TNode>
    {
        private Dictionary<int, TNode> _dict;

        public new TNode RootNode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (TNode)base.RootNode;
        }

        public TNode GetNodeById(int id)
        {
            if (_dict.TryGetValue(id, out TNode value) && value.RealNode())
                return value;

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<int, TNode>.ValueCollection GetNodes()
        {
            return _dict.Values;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _dict = _nodes.ToDictionary(key => key.Id, value => (TNode)value);
        }

        internal IEnumerator<Transition<TNode>> GetEnumeratorFor(RawNode node)
        {
            Transition[] next = node.Next;

            for (int i = 0; i < next.Length; i++)
            {
                RawNode nextNode = _dict[next[i].NextNodeId];

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

        internal override void InitializeMachine<TState, TData>(StateMachine<TState, TData> stateMachine)
        {
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
        }

#if UNITY_EDITOR
        internal sealed override Type GetNodeType() => typeof(TNode);
#endif
    }
}
#endif
