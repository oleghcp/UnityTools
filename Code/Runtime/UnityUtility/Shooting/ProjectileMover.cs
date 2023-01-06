using System;
using UnityEngine;
using UnityUtility.Engine;
using UnityUtility.Mathematics;

#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
namespace UnityUtility.Shooting
{
    [Serializable]
    public struct ProjectileMover
    {
        [SerializeField]
        private bool _useGravity;
        [SerializeField, Min(0f)]
        private float _startSpeed;
        [SerializeField, Range(0f, 1f)]
        private float _moveInInitialFrame;
        [SerializeField]
        private DragOptions _drag;
        [SerializeField]
        private RicochetOptions _ricochets;

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

        public float Darg
        {
            get => _drag.Value;
            set => _drag.Value = value.ClampMin(0f);
        }

#if INCLUDE_PHYSICS
        internal Vector3 GetNextPos(in Vector3 curPos, ref Vector3 velocity, in Vector3 gravity, float deltaTime, float speedScale)
        {
            if (_useGravity)
                velocity += gravity * deltaTime;

            switch (_drag.Method)
            {
                case DragMethod.Linear:
                    Vector3 direction = velocity.GetNormalized(out float speed);
                    velocity = direction * (speed - _drag.Value * deltaTime).ClampMin(0f);
                    break;

                case DragMethod.Exponential:
                    velocity /= 1f + _drag.Value * deltaTime;
                    break;
            }

            return curPos + velocity * (deltaTime * speedScale);
        }

        internal (Vector3 newDest, Vector3 newDir) Reflect(in RaycastHit hitInfo, in Vector3 dest, in Vector3 direction, float castRadius)
        {
            Vector3 newDirection = Vector3.Reflect(direction, hitInfo.normal);
            Vector3 hitPosition = hitInfo.point + hitInfo.normal * castRadius;
            float distanceAfterHit = Vector3.Distance(hitPosition, dest) * _ricochets.SpeedRemainder;

            return (hitPosition + newDirection * distanceAfterHit, newDirection);
        }
#endif

#if INCLUDE_PHYSICS_2D
        internal Vector2 GetNextPos(in Vector2 curPos, ref Vector2 velocity, in Vector2 gravity, float deltaTime, float speedScale)
        {
            if (_useGravity)
                velocity += gravity * deltaTime;

            switch (_drag.Method)
            {
                case DragMethod.Linear:
                    Vector2 direction = velocity.GetNormalized(out float speed);
                    velocity = direction * (speed - _drag.Value * deltaTime).ClampMin(0f);
                    break;

                case DragMethod.Exponential:
                    velocity /= 1f + _drag.Value * deltaTime;
                    break;
            }

            return curPos + velocity * (deltaTime * speedScale);
        }

        internal (Vector2 newDest, Vector2 newDir) Reflect(in RaycastHit2D hitInfo, in Vector2 dest, in Vector2 direction, float castRadius)
        {
            Vector2 newDirection = Vector2.Reflect(direction, hitInfo.normal);
            Vector2 hitPosition = hitInfo.point + hitInfo.normal * castRadius;
            float distanceAfterHit = Vector2.Distance(hitPosition, dest) * _ricochets.SpeedRemainder;

            return (hitPosition + newDirection * distanceAfterHit, newDirection);
        }
#endif
    }
}
#endif
