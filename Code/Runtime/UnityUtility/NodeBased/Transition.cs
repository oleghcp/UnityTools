using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityUtility.NodeBased
{
    [Serializable]
    public class Transition
    {
        [SerializeField, FormerlySerializedAs("NextNode")]
        private ScriptableObject _nextNode;

        public ScriptableObject NextNode => _nextNode;

#if UNITY_EDITOR
        [SerializeField]
        private Vector2[] _points;

        internal static string NodeFieldName => nameof(_nextNode);
        internal static string PointsFieldName => nameof(_points);
#endif
    }
}
