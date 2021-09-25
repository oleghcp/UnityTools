using System;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility.Shooting
{
    [Serializable]
    public class ProjectileMover
    {
        [SerializeField]
        private float _startSpeed;
        [field: SerializeField]
        public bool UseGravity { get; set; }
        [SerializeField, Min(-1)]
        private int _ricochets;
        [field: SerializeField]
        public LayerMask RicochetMask { get; set; }
        [SerializeField, Range(0f, 1f)]
        private float _speedRemainder = 1f;

        public float SpeedRemainder
        {
            get => _speedRemainder;
            set => _speedRemainder = value.Clamp01();
        }

        public int Ricochets
        {
            get => _ricochets;
            set => _ricochets = value.CutBefore(-1);
        }

        public float StartSpeed
        {
            get => _startSpeed;
            set => _startSpeed = value.CutBefore(0f);
        }

        internal Vector3 GetNextPos(in Vector3 curPos, ref Vector3 velocity, in Vector3 gravity, float deltaTime, float speedScale)
        {
            if (UseGravity)
                velocity += gravity * deltaTime;

            return curPos + velocity * (deltaTime * speedScale);
        }

        internal (Vector3 newDest, Vector3 newDir) Reflect(in RaycastHit hitInfo, in Vector3 dest, in Vector3 direction)
        {
            Vector3 newDirection = Vector3.Reflect(direction, hitInfo.normal);
            float distanceAfterHit = Vector3.Distance(hitInfo.point, dest) * _speedRemainder;

            return (hitInfo.point + newDirection * distanceAfterHit, newDirection);
        }
    }
}
