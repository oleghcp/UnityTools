using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    public abstract class RawNode : ScriptableObject, IEnumerable
    {
        [SerializeField]
        internal int Id;
        [SerializeField]
        internal RawGraph Owner;
        [SerializeField]
        internal Transition[] Next;

        public int LocalId => Id;

        public virtual TState CreateState<TState>() where TState : class, IState
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Next.Length; i++)
            {
                RawNode node = Next[i].NextNode;

                if (node is HubNode)
                {
                    Transition[] nextFromHub = node.Next;

                    for (int j = 0; j < nextFromHub.Length; j++)
                    {
                        yield return nextFromHub[j];
                    }

                    continue;
                }

                yield return Next[i];
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        internal Vector2 Position;

        internal static string PositionFieldName => nameof(Position);
        internal static string IdFieldName => nameof(Id);
        internal static string GraphFieldName => nameof(Owner);
        internal static string ArrayFieldName => nameof(Next);
#endif
    }

    public abstract class Node<TNode> : RawNode, IEnumerable<Transition<TNode>> where TNode : Node<TNode>
    {
        public Graph<TNode> Graph => Owner as Graph<TNode>;

        public IEnumerator<Transition<TNode>> GetEnumerator()
        {
            for (int i = 0; i < Next.Length; i++)
            {
                RawNode node = Next[i].NextNode;

                if (node is HubNode)
                {
                    Transition[] nextFromHub = node.Next;

                    for (int j = 0; j < nextFromHub.Length; j++)
                    {
                        yield return new Transition<TNode>(nextFromHub[i]);
                    }

                    continue;
                }

                if (node is ExitNode)
                {
                    yield return new Transition<TNode>(Next[i].CreateExit());
                    continue;
                }

                yield return new Transition<TNode>(Next[i]);
            }
        }
    }
}
