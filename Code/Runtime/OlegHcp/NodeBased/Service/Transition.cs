using System;
using UnityEngine;

namespace OlegHcp.NodeBased.Service
{
    [Serializable]
    internal struct Transition
    {
        [SerializeField]
        private int _nextNodeId;
        [SerializeReference]
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
}
