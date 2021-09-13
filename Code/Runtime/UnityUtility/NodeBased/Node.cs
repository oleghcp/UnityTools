using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    public abstract class RawNode : ScriptableObject
    {
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal RawGraph Owner;
        [SerializeField]
        private protected Transition[] Next;

        public int LocalId => Id;
        public RawGraph Graph => Owner;

        public int NextCount => Next.Length;
        public IReadOnlyList<Transition> Transitions => Next;

        public RawNode GetNextNode(int index)
        {
            return Next[index].NextNode;
        }

        public abstract (bool valid, TState state) CreateState<TState>() where TState : class, IState;

#if UNITY_EDITOR
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string GraphFieldName => nameof(Owner);
        internal static string ArrayFieldName => nameof(Next);
#endif
    }

    public abstract class Node<TNode> : RawNode, IReadOnlyList<TNode> where TNode : Node<TNode>
    {
        public TNode this[int index] => (TNode)Next[index].NextNode;
        public new Graph<TNode> Graph => Owner as Graph<TNode>;
        int IReadOnlyCollection<TNode>.Count => Next.Length;

        public new TNode GetNextNode(int index)
        {
            return (TNode)Next[index].NextNode;
        }

        public override (bool valid, TState state) CreateState<TState>()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TNode> GetEnumerator()
        {
            for (int i = 0; i < NextCount; i++)
            {
                yield return (TNode)Next[i].NextNode;
            };
        }
    }
}
