#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System;
using OlegHcp.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace OlegHcp.Shooting
{
    [Serializable]
    public struct HitOptions
    {
        private const float MIN_SPEED_MULTIPLIER = 0.0001f;

        [SerializeField]
        private HitReactionType _reaction;
        [SerializeField, Min(1f)]
        private int _count;
        [SerializeField, FormerlySerializedAs("_ricochetMask")]
        private LayerMask _mask;
        [SerializeField, HideInInspector]
        private float _speedLoss; // TODO: obsolete parameter, should be removed
        [SerializeField, Min(MIN_SPEED_MULTIPLIER)]
        private float _speedMultiplier;

        private int _left;

#if UNITY_EDITOR
        public static string ReactionFieldName => nameof(_reaction);
        public static string CountFieldName => nameof(_count);
        public static string MaskFieldName => nameof(_mask);
        public static string LossFieldName => nameof(_speedLoss);
        public static string MultiplierFieldName => nameof(_speedMultiplier);
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

        public float SpeedMultiplier
        {
            get => _speedMultiplier > 0f ? _speedMultiplier : 1f - _speedLoss;
            set => _speedMultiplier = value.ClampMin(MIN_SPEED_MULTIPLIER);
        }

        internal int Left => _left;

        public HitOptions(HitReactionType reaction, int count, LayerMask mask, float speedMultiplier)
        {
            _reaction = reaction;
            _count = count.ClampMin(0);
            _mask = mask;
            _speedMultiplier = speedMultiplier;
            _speedLoss = 0;
            _left = 0;
        }

        public bool HasLayer(int layer)
        {
            return BitMask.HasFlag(_mask, layer);
        }

        internal void Reset()
        {
            _left = Count;
        }

        internal void UpdateHit()
        {
            _left--;
        }
    }

    [Serializable]
    internal struct CastOptions
    {
        [Min(0f)]
        public float CastRadius;
    }

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
}
#endif
