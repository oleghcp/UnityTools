using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.NodeBased
{
    public abstract class Graph : ScriptableObject
    {
        [SerializeField]
        internal Node[] Nodes;

        public Node RootNode => Nodes.Length > 0 ? Nodes[0] : null;

#if UNITY_EDITOR
        [SerializeField]
        internal int LastId;
        [SerializeField]
        internal Vector2 CameraPosition;
        [SerializeField]
        private float _nodeWidth;

        internal static string IdGeneratorFieldName => nameof(LastId);
        internal static string WidthFieldName => nameof(_nodeWidth);
        internal static string CameraPositionFieldName => nameof(CameraPosition);
        internal static string ArrayFieldName => nameof(Nodes);
#endif
    }

    public abstract class Graph<TNode, TTransition> : Graph where TNode : Node where TTransition : Transition<TNode>, new()
    {
        public new TNode RootNode => base.RootNode as TNode;

        public TNode GetNodeById(int id)
        {
            return Nodes.Find(item => item.LocalId == id) as TNode;
        }

        public TransitionsInfo<TNode, TTransition> GetTransitons(TNode node)
        {
            return new TransitionsInfo<TNode, TTransition>(node);
        }

#if UNITY_EDITOR
        internal Type GetNodeType() => typeof(TNode);
        internal Type GetTransitionType() => typeof(TTransition);

        internal static string GetNodeTypeMethodName => nameof(GetNodeType);
        internal static string GetTransitionTypeMethodName => nameof(GetTransitionType);
#endif
    }

    public struct TransitionsInfo<TNode, TTransition> : IEnumerable<TTransition> where TNode : Node where TTransition : Transition<TNode>, new()
    {
        private TNode _node;

        public int Count => _node.Next.Length;

        public TTransition this[int index] => _node.Next[index] as TTransition;

        internal TransitionsInfo(TNode node)
        {
            _node = node;
        }

        IEnumerator<TTransition> IEnumerable<TTransition>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<TTransition>
        {
            private readonly TransitionsInfo<TNode, TTransition> _info;
            private int _index;

            public TTransition Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _info[_index];
            }

            object IEnumerator.Current => Current;

            internal Enumerator(TransitionsInfo<TNode, TTransition> info)
            {
                _info = info;
                _index = -1;
            }

            public bool MoveNext()
            {
                int num = _index + 1;
                if (num < _info.Count)
                {
                    _index = num;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _index = 0;
            }

            public void Dispose() { }
        }
    }
}
#endif
