using System;
using UnityEngine;

namespace UnityUtility.Shooting
{
    [Serializable]
    internal class ProjectileMover
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

        public Vector3 GetNextPos(in Vector3 curPos, in Vector3 dir, float curSpeed, float deltaTime, float speedScale)
        {
            if (_useGravity)
                return curPos + (dir * curSpeed + Physics.gravity * deltaTime) * (deltaTime * speedScale);
            else
                return curPos + dir * (curSpeed * deltaTime * speedScale);
        }

        public (Vector3 newDest, Vector3 newDir) Reflect(in RaycastHit hitInfo, in Vector3 dest, Vector3 direction)
        {
            direction = Vector3.Reflect(direction, hitInfo.normal);
            float distanceAfterHit = Vector3.Distance(hitInfo.point, dest) * _speedRemainder;

            return (hitInfo.point + direction * distanceAfterHit, direction);
        }
    }
}
