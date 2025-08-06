#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System;
using OlegHcp.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace OlegHcp.Shooting
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

    public enum HitReactionType
    {
        Ricochet = 0,
        MoveThrough = 1,
    }

    [Serializable]
    public struct HitOptions
    {
        [SerializeField]
        private HitReactionType _reaction;
        [SerializeField]
        private int _count;
        [SerializeField, FormerlySerializedAs("_ricochetMask")]
        private LayerMask _mask;
        [SerializeField, Range(0f, 1f)]
        private float _speedLoss;

        private int _left;

#if UNITY_EDITOR
        public static string ReactionFieldName => nameof(_reaction);
        public static string CountFieldName => nameof(_count);
        public static string MaskFieldName => nameof(_mask);
        public static string LossFieldName => nameof(_speedLoss);
#endif

        public HitReactionType Reaction
        {
            get => _reaction;
            set => _reaction = value;
        }

        public int Count
        {
            get => _count;
            set => _count = value.ClampMin(0);
        }

        public LayerMask Mask
        {
            get => _mask;
            set => _mask = value;
        }

        public float SpeedLoss
        {
            get => _speedLoss;
            set => _speedLoss = value.Clamp01();
        }

        internal float SpeedRemainder => 1f - _speedLoss;
        internal int Left => _left;

        public HitOptions(HitReactionType reaction, int count, LayerMask mask, float speedLoss)
        {
            _reaction = reaction;
            _count = count.ClampMin(0);
            _mask = mask;
            _speedLoss = speedLoss.Clamp01();
            _left = 0;
        }

        internal void Reset()
        {
            _left = Count;
        }

        internal void DecreaseCounter()
        {
            _left--;
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
