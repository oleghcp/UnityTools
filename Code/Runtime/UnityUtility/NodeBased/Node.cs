#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    internal enum NodeType : byte
    {
        Real,
        Hub,
        Exit,
    }

    [Serializable]
    public abstract class RawNode
    {
        [SerializeField]
        internal string NodeName;
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal RawGraph Owner;
        [SerializeField]
        internal Transition[] Next;

        public int LocalId => Id;
        public string Name => NodeName;
        internal virtual NodeType NodeType => NodeType.Real;

        public virtual TState CreateState<TState>() where TState : class, IState
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string NameFieldName => nameof(NodeName);
        internal static string GraphFieldName => nameof(Owner);
        internal static string ArrayFieldName => nameof(Next);
#endif
    }

    [Serializable]
    public abstract class Node<TNode> : RawNode, IEnumerable<Transition<TNode>> where TNode : Node<TNode>
    {
        public Graph<TNode> Graph => Owner as Graph<TNode>;

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this as TNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this as TNode);
        }

        IEnumerator<Transition<TNode>> IEnumerable<Transition<TNode>>.GetEnumerator()
        {
            return new Enumerator(this as TNode);
        }

        public struct Enumerator : IEnumerator<Transition<TNode>>
        {
            private TNode _node;
            private HubNode _hub;
            private int _index;
            private int _subIndex;

            public Transition<TNode> Current
            {
                get
                {
                    int index = (_hub != null ? _subIndex : _index) - 1;
                    Transition[] array = (_hub ?? (RawNode)_node).Next;

                    if ((uint)index >= (uint)array.Length)
                        return default;

                    RawNode nextNode = _node.Owner.Dict[array[index].NextNodeId];
                    return new Transition<TNode>(array[index].Condition, _node, nextNode);
                }
            }

            object IEnumerator.Current => Current;

            internal Enumerator(TNode node)
            {
                _node = node;
                _index = 0;
                _hub = null;
                _subIndex = 0;
            }

            public bool MoveNext()
            {
                if (MoveNext(_hub, ref _subIndex))
                    return true;

                if (MoveNext(_node, ref _index))
                    return true;

                _index = int.MaxValue;
                _subIndex = int.MaxValue;
                _hub = null;
                return false;
            }

            public void Reset()
            {
                _index = 0;
                _hub = null;
                _subIndex = 0;
            }

            public void Dispose() { }

            private bool MoveNext(RawNode node, ref int index)
            {
                if (node == null)
                    return false;

                Transition[] array = node.Next;

                if ((uint)index >= (uint)array.Length)
                    return false;

                Transition transition = array[index];
                RawNode nextNode = node.Owner.Dict[transition.NextNodeId];
                index++;

                if (!(nextNode is HubNode hub))
                    return true;

                _hub = hub;
                _subIndex = 0;

                if (MoveNext(hub, ref _subIndex))
                    return true;

                _hub = null;

                return MoveNext(node, ref index);
            }
        }
    }
}
#endif
