using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
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
                {
                    _nodeList = Dict.Values
                                    .Where(item => item.RealNode())
                                    .Select(item => (TNode)item)
                                    .ToArray();
                }

                return _nodeList;
            }
        }

        public new TNode GetNodeById(int id)
        {
            return base.GetNodeById(id) as TNode;
        }

        public IEnumerableNode<TNode> EnumerateFromAny()
        {
            return _commonNodeWrapper ?? (_commonNodeWrapper = new NodeWrapper(CommonNode));
        }

        //internal override void InitializeMachine<TState, TData>(StateMachine<TState, TData> stateMachine)
        //{
        //    Dictionary<RawNode, TState> states = new Dictionary<RawNode, TState>(Dict.Count);

        //    RawNode rootNode = RootNode;

        //    foreach (RawNode node in Dict.Values)
        //    {
        //        if (node is HubNode)
        //            continue;

        //        TState state = states.Place(node, node.CreateState<TState>());

        //        if (state != null)
        //            stateMachine.AddState(state, node == rootNode);
        //    }

        //    foreach (RawNode node in Dict.Values)
        //    {
        //        if (node.ServiceNode())
        //            continue;

        //        foreach (TransitionInfo<TNode> transition in node as TNode)
        //        {
        //            addTransition(node, transition);
        //        }

        //        foreach (TransitionInfo<TNode> transition in EnumerateFromAny())
        //        {
        //            addTransition(node, transition);
        //        }
        //    }

        //    void addTransition(RawNode node, in TransitionInfo<TNode> transition)
        //    {
        //        stateMachine.AddTransition(states[node],
        //                                   transition.Condition.CreateCondition<TState, TData>(),
        //                                   states[transition.NextNode]);
        //    }
        //}

        public override Type GetNodeRootType()
        {
            return typeof(TNode);
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
