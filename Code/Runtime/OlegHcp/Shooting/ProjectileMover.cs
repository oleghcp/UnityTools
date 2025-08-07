#if INCLUDE_PHYSICS
using System;
using OlegHcp.Engine;
using OlegHcp.Inspector;
using OlegHcp.Mathematics;
using UnityEngine;

namespace OlegHcp.Shooting
{
    [Serializable]
    internal struct ProjectileMover
    {
        #region Inspector
        [SerializeField]
        private bool _useGravity;
        [SerializeField, Min(0f)]
        private float _startSpeed;
        [SerializeField, Range(0f, 1f)]
        private float _moveInInitialFrame;
        [SerializeField, DrawFlags(typeof(LockedAxis))]
        private IntMask _freezePosition;
        [SerializeField]
        private DragOptions _drag;
        #endregion

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

        public IntMask FreezePosition
        {
            get => _freezePosition;
            set => _freezePosition = value;
        }

        public bool HasLocks => 0 != (int)_freezePosition;

        public void LockAxis(int index, bool value)
        {
            _freezePosition[index] = value;
        }

        public Vector3 GetNextPos(in Vector3 curPos, ref Vector3 velocity, in Vector3 gravity, float deltaTime, float speedScale)
        {
            if (_useGravity)
            {
                velocity += gravity * deltaTime;
                if (HasLocks)
                    LockVelocity(ref velocity);
            }

            switch (_drag.Method)
            {
                case DragMethod.Linear:
                    Vector3 direction = velocity.GetNormalized(out float speed);
                    velocity = direction * (speed - _drag.Value * deltaTime).ClampMin(0f);
                    break;

                case DragMethod.NonLinear:
                    velocity /= 1f + _drag.Value * deltaTime;
                    break;
            }

            return curPos + velocity * (deltaTime * speedScale);
        }

        public (Vector3 newDest, Vector3 newDir, Vector3 hitPos) Reflect(in RaycastHit hitInfo, in Vector3 destination, in Vector3 direction, float castRadius, float speedRemainder)
        {
            Vector3 newDirection = Vector3.Reflect(direction, hitInfo.normal);

            bool locked = HasLocks;
            if (locked)
            {
                LockVelocity(ref newDirection);
                newDirection = newDirection.GetNormalized(out float prevMagnitude);
                if (prevMagnitude <= MathUtility.kEpsilon)
                    newDirection = -direction;
            }

            Vector3 hitPosition = GetHitPosition(hitInfo, castRadius);
            float distanceAfterHit = Vector3.Distance(hitPosition, destination) * speedRemainder;
            Vector3 newDest = hitPosition + newDirection * distanceAfterHit;

            if (locked)
                LockPosition(ref newDest, destination);

            return (newDest, newDirection, hitPosition);
        }

        public (Vector3 newDest, Vector3 hitPos) Penetrate(in RaycastHit hitInfo, in Vector3 destination, in Vector3 direction, float castRadius, float speedRemainder)
        {
            Vector3 hitPosition = GetHitPosition(hitInfo, castRadius);

            if (speedRemainder == 1f)
                return (destination, hitPosition);

            float distanceAfterHit = Vector3.Distance(hitPosition, destination) * speedRemainder;
            return (hitPosition + direction * distanceAfterHit, hitPosition);
        }

        public Vector3 GetHitPosition(in RaycastHit hitInfo, float castRadius)
        {
            if (castRadius <= MathUtility.kEpsilon)
                return hitInfo.point;

            return hitInfo.point + hitInfo.normal * castRadius;
        }

        public void LockVelocity(ref Vector3 velocity)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_freezePosition[i])
                    velocity[i] = 0f;
            }
        }

        private void LockPosition(ref Vector3 newDest, in Vector3 refPosition)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_freezePosition[i])
                    newDest[i] = refPosition[i];
            }
        }

        private enum LockedAxis { X, Y, Z, }
    }
}
#endif
