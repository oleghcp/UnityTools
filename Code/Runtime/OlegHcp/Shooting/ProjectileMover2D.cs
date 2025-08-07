#if INCLUDE_PHYSICS_2D
using System;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp.Shooting
{
    [Serializable]
    internal struct ProjectileMover2D
    {
        [SerializeField]
        private bool _useGravity;
        [SerializeField, Min(0f)]
        private float _startSpeed;
        [SerializeField, Range(0f, 1f)]
        private float _moveInInitialFrame;
        [SerializeField]
        private DragOptions _drag;

        public bool UseGravity
        {
            get => _useGravity;
            set => _useGravity = value;
        }

        public float StartSpeed
        {
            get => _startSpeed;
            set => _startSpeed = value.ClampMin(0f);
        }

        public float MoveInInitialFrame
        {
            get => _moveInInitialFrame;
            set => _moveInInitialFrame = value.Clamp01();
        }

        public DragMethod DragMethod
        {
            get => _drag.Method;
            set => _drag.Method = value;
        }

        public float Drag
        {
            get => _drag.Value;
            set => _drag.Value = value.ClampMin(0f);
        }

        public Vector2 GetNextPos(in Vector2 curPos, ref Vector2 velocity, in Vector2 gravity, float deltaTime, float speedScale)
        {
            if (_useGravity)
                velocity += gravity * deltaTime;

            switch (_drag.Method)
            {
                case DragMethod.Linear:
                    Vector2 direction = velocity.GetNormalized(out float speed);
                    velocity = direction * (speed - _drag.Value * deltaTime).ClampMin(0f);
                    break;

                case DragMethod.NonLinear:
                    velocity /= 1f + _drag.Value * deltaTime;
                    break;
            }

            return curPos + velocity * (deltaTime * speedScale);
        }

        public (Vector2 newDest, Vector2 newDir, Vector2 hitPos) Reflect(in RaycastHit2D hitInfo, in Vector2 dest, in Vector2 direction, float castRadius, float speedMultiplier)
        {
            Vector2 newDirection = Vector2.Reflect(direction, hitInfo.normal);
            Vector2 hitPosition = GetHitPosition(hitInfo, castRadius);
            float distanceAfterHit = Vector2.Distance(hitPosition, dest) * speedMultiplier;

            return (hitPosition + newDirection * distanceAfterHit, newDirection, hitPosition);
        }

        public (Vector2 newDest, Vector2 hitPos) Penetrate(in RaycastHit2D hitInfo, in Vector2 destination, in Vector2 direction, float castRadius, float speedMultiplier)
        {
            Vector2 hitPosition = GetHitPosition(hitInfo, castRadius);

            if (speedMultiplier == 1f)
                return (destination, hitPosition);

            float distanceAfterHit = Vector2.Distance(hitPosition, destination) * speedMultiplier;
            return (hitPosition + direction * distanceAfterHit, hitPosition);
        }

        public Vector2 GetHitPosition(in RaycastHit2D hitInfo, float castRadius)
        {
            if (castRadius <= MathUtility.kEpsilon)
                return hitInfo.point;

            return hitInfo.point + hitInfo.normal * castRadius;
        }
    }
}
#endif
