using System;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
namespace UnityUtility.Shooting
{
    [Serializable]
    internal struct Debugger
    {
        public bool DrawPath;
        [Min(1f)]
        public float Duration;
        public Color Color;

        public void Draw(in Vector3 from, in Vector3 to)
        {
            if (DrawPath)
                Debug.DrawLine(from, to, Color, Duration);
        }
    }
}
#endif
