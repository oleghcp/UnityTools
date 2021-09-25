using System;
using UnityEngine;

namespace UnityUtility.Shooting
{
    [Serializable]
    internal class ProjectileMover2D
    {
        [SerializeField]
        private float _startSpeed;
        [SerializeField]
        private bool _useGravity;
        [SerializeField, Min(-1)]
        private int _ricochets;
        [SerializeField]
        private LayerMask _ricochetMask;
        [SerializeField, Range(0f, 1f)]
        private float _speedRemainder = 1f;

        public float StartSpeed => _startSpeed;
        public int Ricochets => _ricochets;
        public float SpeedRemainder => _speedRemainder;
        public LayerMask RicochetMask => _ricochetMask;

        public Vector2 GetNextPos(in Vector2 curPos, ref Vector2 velocity, in Vector2 gravity, float deltaTime, float speedScale)
        {
            if (_useGravity)
                velocity += gravity * deltaTime;

            return curPos + velocity * (deltaTime * speedScale);
        }

        public (Vector2 newDest, Vector2 newDir) Reflect(in RaycastHit2D hitInfo, in Vector2 dest, in Vector2 direction)
        {
            Vector2 newDirection = Vector2.Reflect(direction, hitInfo.normal);
            float distanceAfterHit = Vector2.Distance(hitInfo.point, dest) * _speedRemainder;

            return (hitInfo.point + newDirection * distanceAfterHit, newDirection);
        }
    }
}
