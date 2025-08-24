#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp.Shooting
{
    [Serializable]
    internal struct ProjectileCaster
    {
        [SerializeField]
        private CastOptions _castRadius;
        [SerializeField]
        private LayerMask _hitMask;
        [SerializeField, Min(0f)]
        private float _initialPrecastBackOffset;

        public float CastRadius
        {
            get => _castRadius.CastRadius;
            set => _castRadius.CastRadius = value.ClampMin(0f);
        }

        public LayerMask HitMask
        {
            get => _hitMask;
            set => _hitMask = value;
        }

        public float InitialPrecastBackOffset
        {
            get => _initialPrecastBackOffset;
            set => _initialPrecastBackOffset = value.ClampMin(0f);
        }

#if INCLUDE_PHYSICS
        public bool Cast(in Vector3 source, in Vector3 direction, float distance, out RaycastHit hitInfo)
        {
            if (_castRadius.CastRadius > float.Epsilon)
                return Physics.SphereCast(source, _castRadius.CastRadius, direction, out hitInfo, distance, _hitMask);

            return Physics.Raycast(source, direction, out hitInfo, distance, _hitMask);
        }
#endif

#if INCLUDE_PHYSICS_2D
        public bool Cast(in Vector2 source, in Vector2 direction, float distance, out RaycastHit2D hitInfo)
        {
            if (_castRadius.CastRadius > float.Epsilon)
            {
                hitInfo = Physics2D.CircleCast(source, _castRadius.CastRadius, direction, distance, _hitMask);
                return hitInfo.Hit();
            }

            hitInfo = Physics2D.Raycast(source, direction, distance, _hitMask);
            return hitInfo.Hit();
        }
#endif
    }
}
#endif
