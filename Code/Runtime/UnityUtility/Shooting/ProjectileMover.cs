#if INCLUDE_PHYSICS
using System;
using UnityEngine;
using UnityUtility.Engine;
using UnityUtility.Inspector;
using UnityUtility.Mathematics;

namespace UnityUtility.Shooting
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
        [SerializeField]
        private RicochetOptions _ricochets;
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

        public int Ricochets
        {
            get => _ricochets.Count;
            set => _ricochets.Count = value.ClampMin(0);
        }

        public float SpeedRemainder
        {
            get => _ricochets.SpeedRemainder;
            set => _ricochets.SpeedRemainder = value.Clamp01();
        }

        public float MoveInInitialFrame
        {
            get => _moveInInitialFrame;
            set => _moveInInitialFrame = value.Clamp01();
        }

        public LayerMask RicochetMask
        {
            get => _ricochets.RicochetMask;
            set => _ricochets.RicochetMask = value;
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

        public (Vector3 newDest, Vector3 newDir) Reflect(in RaycastHit hitInfo, in Vector3 dest, in Vector3 direction, float castRadius)
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
            float distanceAfterHit = Vector3.Distance(hitPosition, dest) * _ricochets.SpeedRemainder;
            Vector3 newDest = hitPosition + newDirection * distanceAfterHit;

            if (locked)
                LockPosition(ref newDest, dest);

            return (newDest, newDirection);
        }

        public Vector3 GetHitPosition(in RaycastHit hitInfo, float castRadius)
        {
            if (castRadius <= MathUtility.kEpsilon)
                return hitInfo.point;

            return hitInfo.point + hitInfo.normal * castRadius;
        }

        public void LockVelocity(ref Vector3 veclocity)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_freezePosition[i])
                    veclocity[i] = 0f;
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
