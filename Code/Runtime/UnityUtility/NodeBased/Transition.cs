using System;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.NodeBased
{
    [Serializable]
    public class Transition
    {
        [SerializeField]
        internal ScriptableObject NextNode;

#if UNITY_EDITOR
        [SerializeField]
        private Vector2[] _points;

        internal static string NodeFieldName => nameof(NextNode);
        internal static string PointsFieldName => nameof(_points);
#endif
    }
}
#endif
