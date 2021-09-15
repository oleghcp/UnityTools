using System;
using UnityEngine;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    [Serializable]
    internal struct Transition
    {
        [SerializeField]
        private RawNode _nextNode;
        [SerializeReference, ReferenceSelection]
        private Condition _condition;

        public RawNode NextNode => _nextNode;

        public Transition CreateExit()
        {
            return new Transition
            {
                _condition = _condition,
            };
        }

        public bool Available(object data = null)
        {
            return _condition == null || _condition.Satisfied(_nextNode, data);
        }

        public Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState where TData : class
        {
            return _condition.CreateCondition<TState, TData>();
        }

#if UNITY_EDITOR
        [SerializeField]
        private Vector2[] _points;

        internal static string NodeFieldName => nameof(_nextNode);
        internal static string ConditionFieldName => nameof(_condition);
        internal static string PointsFieldName => nameof(_points);
#endif
    }

    public struct Transition<TNode> where TNode : Node<TNode>
    {
        private Transition _transition;

        public TNode NextNode => _transition.NextNode as TNode;
        public bool IsExit => _transition.NextNode is ExitNode;

        internal Transition(in Transition transition)
        {
            _transition = transition;
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
        public abstract bool Satisfied(RawNode nextNode, object data);

        public virtual Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState where TData : class
        {
            throw new NotImplementedException();
        }
    }
}
