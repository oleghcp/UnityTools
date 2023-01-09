using System;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
namespace UnityUtility.Shooting
{
    [Serializable]
    internal struct DragOptions
    {
        public DragMethod Method;
        [Min(0f)]
        public float Value;
    }

    public enum DragMethod
    {
        None = 0,
        Linear = 1,
        NonLinear = 2,
    }

    [Serializable]
    internal struct RicochetOptions
    {
        public int Count;
        public LayerMask RicochetMask;
        [Range(0f, 1f)]
        public float SpeedRemainder;
    }

    [Serializable]
    internal struct CastOptions
    {
        [Min(0f)]
        public float CastRadius;
        public bool HighPrecision;
    }
}
#endif
