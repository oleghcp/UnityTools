#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System;
using UnityEngine;

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

        private int _ricochetsLeft;

        public int RicochetsLeft => _ricochetsLeft;

        internal void ResetRicochets()
        {
            _ricochetsLeft = Count;
        }

        internal void DecreaseCounter()
        {
            _ricochetsLeft--;
        }
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
