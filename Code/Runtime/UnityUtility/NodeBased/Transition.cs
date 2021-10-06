#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    [Serializable]
    internal struct Transition
    {
        [SerializeField]
        private int _nextNodeId;
        [SerializeReference, ReferenceSelection]
        private Condition _condition;

        public int NextNodeId => _nextNodeId;
        public Condition Condition => _condition;

#if UNITY_EDITOR
        [SerializeField]
        private Vector2[] _points;

        internal static string NodeIdFieldName => nameof(_nextNodeId);
        internal static string ConditionFieldName => nameof(_condition);
        internal static string PointsFieldName => nameof(_points);
#endif
    }

    public struct Transition<TNode> where TNode : Node<TNode>
    {
        private Condition _condition;
        private RawNode _owner;
        private RawNode _nextNode;

        public TNode NextNode => _nextNode as TNode;
        public bool IsExit => _nextNode is ExitNode;

        internal Transition(Condition condition, RawNode from, RawNode to)
        {
            _condition = condition;
            _owner = from;
            _nextNode = to;
        }

        public bool Available(object data = null)
        {
            return _condition == null || _condition.Satisfied(_owner, data);
        }

        public Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState
        {
            return _condition.CreateCondition<TState, TData>();
        }
    }

    [Serializable]
    public abstract class Condition
    {
        public abstract bool Satisfied(RawNode from, object data);

        public virtual Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState
        {
            throw new NotImplementedException();
        }
    }
}
#endif
