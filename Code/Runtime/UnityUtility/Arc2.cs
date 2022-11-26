using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityUtility.Mathematics;
using static System.MathF;

namespace UnityUtility
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public struct Arc2
    {
        /// <summary>
        /// Start launch angle in degrees.
        /// </summary>
        public float StartAngle;

        /// <summary>
        /// Start launch speed.
        /// </summary>
        public float StartSpeed;

        /// <summary>
        /// Gravity.
        /// </summary>
        public float Gravity;

        /// <summary>
        /// Start launch position.
        /// </summary>
        public Vector2 StartPos;

        /// <summary>
        /// Start launch direction (normalized).
        /// </summary>
        public Vector2 StartDir
        {
            get
            {
                float angle = StartAngle.ToRadians();
                return new Vector2(Cos(angle), Sin(angle));
            }
            set => StartAngle = DirToAngle(value);
        }

        /// <summary>
        /// Start launch direction vector with magnitude equals to StartSpeed.
        /// </summary>
        public Vector2 StartVector
        {
            get => StartDir * StartSpeed;
            set
            {
                StartAngle = DirToAngle(value);
                StartSpeed = value.magnitude;
            }
        }

        public Arc2(float angle, float startSpeed, float gravity, Vector2 startPos = default)
        {
            StartAngle = angle;
            StartSpeed = startSpeed;
            Gravity = gravity;
            StartPos = startPos;
        }

        public Arc2(Vector2 dir, float startSpeed, float gravity, Vector2 startPos = default)
        {
            StartAngle = DirToAngle(dir);
            StartSpeed = startSpeed;
            Gravity = gravity;
            StartPos = startPos;
        }

        public Vector2 Evaluate(float time)
        {
            return GetArcPos(StartAngle, StartSpeed, Gravity, time) + StartPos;
        }

        public Vector2 Evaluate(float time, float wind)
        {
            return GetArcPos(StartAngle, StartSpeed, Gravity, wind, time) + StartPos;
        }

        // -- //

        internal static Vector2 GetArcPos(float startAngle, float startSpeed, float gravity, float time)
        {
            float angle = startAngle.ToRadians();
            float x = startSpeed * time * Cos(angle);
            float y = (startSpeed * Sin(angle) - gravity * time * 0.5f) * time;
            return new Vector2(x, y);
        }

        internal static Vector2 GetArcPos(float startAngle, float startSpeed, float gravity, float wind, float time)
        {
            float angle = startAngle.ToRadians();
            float x = (startSpeed * Cos(angle) + wind * time * 0.5f) * time;
            float y = (startSpeed * Sin(angle) - gravity * time * 0.5f) * time;
            return new Vector2(x, y);
        }

        private static float DirToAngle(Vector2 dir)
        {
            float angle = Vector3.Angle(Vector2.right, dir);
            return dir.y >= 0f ? angle : -angle;
        }
    }
}
