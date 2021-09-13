using System;
using UnityEngine;
using UnityEngine.Serialization;

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
            return _condition == null ? true : _condition.Satisfied(this, data);
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
    }
}
