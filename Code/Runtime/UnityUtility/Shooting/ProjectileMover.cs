using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityUtility.MathExt;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProjectileMover
    {
        [SerializeField]
        private float _startSpeed;
        [field: SerializeField]
        public bool UseGravity { get; set; }
        [SerializeField]
        private RicochetOptions _ricochets;

        public float StartSpeed
        {
            get => _startSpeed;
            set => _startSpeed = value.CutBefore(0f);
        }

        public int Ricochets
        {
            get => _ricochets.Count;
            set => _ricochets.Count = value.CutBefore(0);
        }

        public float SpeedRemainder
        {
            get => _ricochets.SpeedRemainder;
            set => _ricochets.SpeedRemainder = value.Clamp01();
        }

        public LayerMask RicochetMask
        {
            get => _ricochets.RicochetMask;
            set => _ricochets.RicochetMask = value;
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
            float distanceAfterHit = Vector3.Distance(hitInfo.point, dest) * _ricochets.SpeedRemainder;

            return (hitInfo.point + newDirection * distanceAfterHit, newDirection);
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
            float distanceAfterHit = Vector2.Distance(hitInfo.point, dest) * _ricochets.SpeedRemainder;

            return (hitInfo.point + newDirection * distanceAfterHit, newDirection);
        }
    }
}
#endif
