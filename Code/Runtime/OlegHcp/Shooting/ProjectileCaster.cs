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
        [SerializeField, Min(0f)]
        private float _initialPrecastBackOffset;
        [SerializeField]
        private LayerMask _hitMask;
        [SerializeField]
        private QueryTriggerInteraction _triggerInteraction;

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
        public bool Cast(in Vector3 origin, in Vector3 direction, float distance, out RaycastHit hitInfo)
        {
            if (_castRadius.CastRadius > MathUtility.kEpsilon)
                return Physics.SphereCast(origin, _castRadius.CastRadius, direction, out hitInfo, distance, _hitMask, _triggerInteraction);

            return Physics.Raycast(origin, direction, out hitInfo, distance, _hitMask, _triggerInteraction);
        }
#endif

#if INCLUDE_PHYSICS_2D
        public bool Cast(in Vector2 origin, in Vector2 direction, float distance, out RaycastHit2D hitInfo)
        {
            ContactFilter2D contactFilter = default;
            contactFilter.useTriggers = UseTriggers(_triggerInteraction);
            contactFilter.SetLayerMask(_hitMask);
            contactFilter.SetDepth(float.NegativeInfinity, float.PositiveInfinity);

            if (_castRadius.CastRadius > MathUtility.kEpsilon)
            {
                hitInfo = Physics2D.defaultPhysicsScene.CircleCast(origin, _castRadius.CastRadius, direction, distance, contactFilter);
                return hitInfo.Hit();
            }

            hitInfo = Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter);
            return hitInfo.Hit();
        }

        private static bool UseTriggers(QueryTriggerInteraction triggerInteraction)
        {
            switch (triggerInteraction)
            {
                case QueryTriggerInteraction.Ignore: return false;
                case QueryTriggerInteraction.Collide: return true;
                default: return Physics2D.queriesHitTriggers;
            }
        }
#endif
    }
}
#endif
