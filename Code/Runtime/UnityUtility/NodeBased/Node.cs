using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility.NodeBased
{
    public abstract class RawNode : ScriptableObject
    {
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal RawGraph Owner;

        public int LocalId => Id;
        public RawGraph Graph => Owner;

#if UNITY_EDITOR
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string GraphFieldName => nameof(Owner);
#endif
    }

    public abstract class Node<TTransition> : RawNode where TTransition : Transition, new()
    {
        [SerializeField]
        private protected TTransition[] Next;

        public int NextCount => Next.Length;
        public IReadOnlyList<TTransition> Transitions => Next;

        public Node<TTransition> GetNextNode(int index)
        {
            return (Node<TTransition>)Next[index].NextNode;
        }

#if UNITY_EDITOR
        internal static string ArrayFieldName => nameof(Next);
#endif
    }

    public abstract class SimpleNode<T> : Node<Transition>, IReadOnlyList<T> where T : SimpleNode<T>
    {
        public T this[int index] => (T)Next[index].NextNode;
        public new Graph<T, Transition> Graph => Owner as Graph<T, Transition>;
        int IReadOnlyCollection<T>.Count => Next.Length;

        public new T GetNextNode(int index)
        {
            return (T)Next[index].NextNode;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < NextCount; i++)
            {
                yield return (T)Next[i].NextNode;
            };
        }
    }
}
