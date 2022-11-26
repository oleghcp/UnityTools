using System;
using UnityEngine;
using UnityUtility.Mathematics;

#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
namespace UnityUtility.Shooting
{
    [Serializable]
    public struct ProjectileCaster
    {
        [SerializeField]
        private CastOptions _castRadius;
        [SerializeField]
        private LayerMask _hitMask;
        [SerializeField]
        private float _initialPrecastOffset;
        [SerializeField, Range(0f, 1f)]
        private float _reflectedCastNear;

        public float CastRadius
        {
            get => _castRadius.CastRadius;
            set => _castRadius.CastRadius = value.ClampMin(0f);
        }

        public bool HighPrecision
        {
            get => _castRadius.HighPrecision;
            set => _castRadius.HighPrecision = value;
        }

        public LayerMask HitMask
        {
            get => _hitMask;
            set => _hitMask = value;
        }

        public float ReflectedCastNear
        {
            get => _reflectedCastNear;
            set => _reflectedCastNear = value.Clamp01();
        }

        public float InitialPrecastOffset
        {
            get => _initialPrecastOffset;
            set => _initialPrecastOffset = value.ClampMax(0f);
        }

#if INCLUDE_PHYSICS
        internal bool Cast(in Vector3 source, in Vector3 direction, float distance, out RaycastHit hitInfo)
        {
            if (_castRadius.CastRadius > float.Epsilon)
            {
                bool hit = Physics.SphereCast(source, _castRadius.CastRadius, direction, out hitInfo, distance, _hitMask);

                if (!_castRadius.HighPrecision)
                    return hit;
                else if (hit)
                    return true;
            }

            return Physics.Raycast(source, direction, out hitInfo, distance, _hitMask);
        }
#endif

#if INCLUDE_PHYSICS_2D
        internal bool Cast(in Vector2 source, in Vector2 direction, float distance, out RaycastHit2D hitInfo)
        {
            if (_castRadius.CastRadius > float.Epsilon)
            {
                hitInfo = Physics2D.CircleCast(source, _castRadius.CastRadius, direction, distance, _hitMask);

                if (!_castRadius.HighPrecision)
                    return hitInfo.Hit();
                else if (hitInfo.Hit())
                    return true;
            }

            hitInfo = Physics2D.Raycast(source, direction, distance, _hitMask);
            return hitInfo.Hit();
        }
#endif
    }
}
#endif
