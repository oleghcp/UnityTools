#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System;
using UnityEngine;
using UnityUtility.Mathematics;

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
        public float SpeedLoss;

        private int _ricochetsLeft;

        internal float SpeedRemainder => 1f - SpeedLoss;
        public int RicochetsLeft => _ricochetsLeft;

        public RicochetOptions(LayerMask mask, int count, float speedLoss)
        {
            Count = count;
            RicochetMask = mask;
            SpeedLoss = speedLoss.Clamp01();
            _ricochetsLeft = count;
        }

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
