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

        public bool Available(object data = null)
        {
            return _condition == null || _condition.Satisfied(data);
        }

        public Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState where TData : class
        {
            return _condition.CreateCondition<TState, TData>();
        }

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
        private Transition _transition;
        private RawNode _nextNode;

        public TNode NextNode => _nextNode as TNode;
        public bool IsExit => _nextNode is ExitNode;

        internal Transition(in Transition transition, RawNode nextNode)
        {
            _transition = transition;
            _nextNode = nextNode;
        }

        public bool Available(object data = null)
        {
            return _transition.Available(data);
        }

        public Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState where TData : class
        {
            return _transition.CreateCondition<TState, TData>();
        }
    }

    [Serializable]
    public abstract class Condition
    {
        public abstract bool Satisfied(object data);

        public virtual Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState where TData : class
        {
            throw new NotImplementedException();
        }
    }
}
