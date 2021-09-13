using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtility.Collections;

namespace UnityUtility.NodeBased
{
    [Serializable]
    public struct Transition
    {
        [SerializeField, FormerlySerializedAs("NextNode")]
        private RawNode _nextNode;
        [SerializeReference, ReferenceSelection]
        private Condition _condition;

        public RawNode NextNode => _nextNode;

        public bool Available(object data = null)
        {
            return _condition == null || _condition.Satisfied(this, data);
        }

        public Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState where TData : class
        {
            return _condition.CreateCondition<TState, TData>();
        }

#if UNITY_EDITOR
        [SerializeField]
        private Vector2[] _points;

        internal static string NodeFieldName => nameof(_nextNode);
        internal static string PointsFieldName => nameof(_points);
#endif
    }

    [Serializable]
    public abstract class Condition
    {
        public abstract bool Satisfied(in Transition transition, object data);

        public virtual Func<TState, TData, bool> CreateCondition<TState, TData>() where TState : class, IState where TData : class
        {
            throw new NotImplementedException();
        }
    }
}
