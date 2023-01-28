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
        [SerializeField]
        private int _count;
        [SerializeField]
        private LayerMask _ricochetMask;
        [SerializeField, Range(0f, 1f)]
        private float _speedLoss;

        private int _ricochetsLeft;

#if UNITY_EDITOR
        public static string CountFieldName => nameof(_count);
        public static string MaskFieldName => nameof(_ricochetMask);
        public static string LossFieldName => nameof(_speedLoss);
#endif

        internal float SpeedRemainder => 1f - SpeedLoss;
        public int Count => _count;
        public LayerMask RicochetMask => _ricochetMask;
        public float SpeedLoss => _speedLoss;
        public int RicochetsLeft => _ricochetsLeft;

        public RicochetOptions(LayerMask mask, int count, float speedLoss)
        {
            _count = count;
            _ricochetMask = mask;
            _speedLoss = speedLoss.Clamp01();
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
