using System;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.NodeBased
{
    [Serializable]
    public abstract class Transition
    {
        [SerializeField]
        internal Node Node;

#if UNITY_EDITOR
        [SerializeField]
        private Vector2[] _points;

        internal static string NodeFieldName => nameof(Node);
        internal static string PointsFieldName => nameof(_points);
#endif
    }

    public abstract class Transition<TNode> : Transition where TNode : Node
    {
        public TNode NextNode => Node as TNode;
    }
}
#endif
