using System;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
namespace UnityUtility.Shooting
{
    [Serializable]
    internal class Debugger
    {
        [Min(1f)]
        public float Duration = 15f;
        public Color Color = Colours.Green;

        public void Draw(in Vector3 from, in Vector3 to)
        {
            Debug.DrawLine(from, to, Color, Duration);
        }
    }
}
#endif
