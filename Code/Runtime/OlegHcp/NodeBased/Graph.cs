using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.Collections;
using OlegHcp.CSharp.Collections;
using OlegHcp.NodeBased.Service;

namespace OlegHcp.NodeBased
{
    public abstract class Graph<TNode> : RawGraph where TNode : Node<TNode>
    {
        private TNode[] _nodeList;
        private NodeWrapper _commonNodeWrapper;

        public new TNode RootNode => (TNode)base.RootNode;
        internal sealed override Type RootNodeType => typeof(TNode);

        public IReadOnlyList<TNode> Nodes
        {
            get
            {
                if (_nodeList == null)
                    _nodeList = Dict.Where(item => item.Value.IsRegular())
                                    .Select(item => (TNode)item.Value)
                                    .ToArray();
                return _nodeList;
            }
        }

        public new TNode GetNodeById(int id)
        {
            return base.GetNodeById(id) as TNode;
        }

        public override Type GetNodeRootType()
        {
            return typeof(TNode);
        }

        public IEnumerableNode<TNode> EnumerateFromAny()
        {
            return _commonNodeWrapper ?? (_commonNodeWrapper = new NodeWrapper(CommonNode));
        }

        internal override void InitializeStateMachine<TState, TData>(StateMachine<TState, TData> stateMachine)
        {
            Dictionary<TNode, TState> states = createStates<TState>();

            stateMachine.SetAsStart(states[RootNode]);

            foreach (var (node, state) in states)
            {
                foreach (TransitionInfo<TNode> transition in node)
                {
                    addTransition(states, stateMachine, state, node, transition);
                }

                foreach (TransitionInfo<TNode> transition in EnumerateFromAny())
                {
                    addTransition(states, stateMachine, state, node, transition);
                }
            }
        }

        private Dictionary<TNode, TState> createStates<TState>() where TState : class, IState
        {
            Dictionary<TNode, TState> dict = new Dictionary<TNode, TState>();

            for (int i = 0; i < NodeArray.Length; i++)
            {
                if (NodeArray[i].IsRegular())
                {
                    TNode node = (TNode)NodeArray[i];
                    dict.Add(node, node.CreateState<TState>());
                }
            }

            return dict;
        }

        private void addTransition<TState, TData>(Dictionary<TNode, TState> states,
                                                  StateMachine<TState, TData> stateMachine,
                                                  TState state,
                                                  TNode node,
                                                  in TransitionInfo<TNode> transition) where TState : class, IState
        {
            var condition = node.CreateCondition<TData>(transition);

            TState nextState = null;

            if (transition.NextNode != null)
                states.TryGetValue(transition.NextNode, out nextState);

            stateMachine.AddTransition(state, condition, nextState);
        }

        private class NodeWrapper : IEnumerableNode<TNode>
        {
            private RawNode _rawNode;

            public NodeWrapper(RawNode rawNode)
            {
                _rawNode = rawNode;
            }

            public NodeEnumerator<TNode> GetEnumerator()
            {
                return new NodeEnumerator<TNode>(_rawNode);
            }

            IEnumerator<TransitionInfo<TNode>> IEnumerable<TransitionInfo<TNode>>.GetEnumerator()
            {
                return new NodeEnumerator<TNode>(_rawNode);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new NodeEnumerator<TNode>(_rawNode);
            }
        }
    }
}
