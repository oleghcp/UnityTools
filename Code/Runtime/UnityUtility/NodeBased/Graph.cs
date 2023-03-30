using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityUtility.Collections;
using UnityUtility.CSharp.Collections;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
{
    public abstract class Graph<TNode> : RawGraph where TNode : Node<TNode>
    {
        private ReadOnlyCollection<TNode> _nodeList;
        private CommonNodeWrapper _commonNodeWrapper;

        public new TNode RootNode => (TNode)base.RootNode;

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

        public IEnumerableNode<TNode> EnumerateFromAny()
        {
            return _commonNodeWrapper ?? (_commonNodeWrapper = new CommonNodeWrapper(CommonNode));
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
                    addTransition(node, transition);
                }

                foreach (Transition<TNode> transition in EnumerateFromAny())
                {
                    addTransition(node, transition);
                }
            }

            void addTransition(RawNode node, in Transition<TNode> transition)
            {
                stateMachine.AddTransition(states[node],
                                           transition.CreateCondition<TState, TData>(),
                                           states[transition.NextNode]);
            }
        }

#if UNITY_EDITOR
        internal sealed override Type GetNodeType() => typeof(TNode);
#endif

        private class CommonNodeWrapper : IEnumerableNode<TNode>
        {
            private RawNode _commonNode;

            public CommonNodeWrapper(RawNode commonNode)
            {
                _commonNode = commonNode;
            }

            public NodeEnumerator<TNode> GetEnumerator()
            {
                return new NodeEnumerator<TNode>(_commonNode);
            }

            IEnumerator<Transition<TNode>> IEnumerable<Transition<TNode>>.GetEnumerator()
            {
                return new NodeEnumerator<TNode>(_commonNode);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new NodeEnumerator<TNode>(_commonNode);
            }
        }
    }
}
