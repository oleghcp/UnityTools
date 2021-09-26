using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProjectileMover2D
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
        private float _speedRemainder;

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

        internal Vector2 GetNextPos(in Vector2 curPos, ref Vector2 velocity, in Vector2 gravity, float deltaTime, float speedScale)
        {
            if (UseGravity)
                velocity += gravity * deltaTime;

            return curPos + velocity * (deltaTime * speedScale);
        }

        internal (Vector2 newDest, Vector2 newDir) Reflect(in RaycastHit2D hitInfo, in Vector2 dest, in Vector2 direction)
        {
            Vector2 newDirection = Vector2.Reflect(direction, hitInfo.normal);
            float distanceAfterHit = Vector2.Distance(hitInfo.point, dest) * _speedRemainder;

            return (hitInfo.point + newDirection * distanceAfterHit, newDirection);
        }
    }
}
