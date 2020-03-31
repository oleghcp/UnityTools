using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityUtility
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Arc3
    {
        /// <summary>
        /// Start launch horizontal angle in degrees.
        /// </summary>
        public float HorAngle;

        /// <summary>
        /// Start launch vertical angle in degrees.
        /// </summary>
        public float VertAngle;

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
        public Vector3 StartPos;

        /// <summary>
        /// Start launch direction (normalized).
        /// </summary>
        public Vector3 StartDir
        {
            get { return f_angleToDir(HorAngle, VertAngle); }
            set { f_dirToAngle(value, out HorAngle, out VertAngle); }
        }

        /// <summary>
        /// Start launch direction vector with magnitude equals to StartSpeed.
        /// </summary>
        public Vector3 StartVector
        {
            get { return f_angleToDir(HorAngle, VertAngle) * StartSpeed; }
            set
            {
                f_dirToAngle(value, out HorAngle, out VertAngle);
                StartSpeed = value.magnitude;
            }
        }

        public Arc3(float vertAngle, float horAngle, float startSpeed, float gravity, in Vector3 startPos = default)
        {
            VertAngle = vertAngle;
            HorAngle = horAngle;
            StartSpeed = startSpeed;
            Gravity = gravity;
            StartPos = startPos;
        }

        public Arc3(Vector3 dir, float startSpeed, float gravity, in Vector3 startPos = default)
        {
            f_dirToAngle(dir, out HorAngle, out VertAngle);
            StartSpeed = startSpeed;
            Gravity = gravity;
            StartPos = startPos;
        }

        public Vector3 Evaluate(float time)
        {
            Vector3 newPos = Arc2.GetArcPos(VertAngle, StartSpeed, Gravity, time);
            return newPos.GetRotated(Vector3.up, HorAngle) + StartPos;
        }

        // -- //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 f_angleToDir(float hor, float vert)
        {
            return Vector3.right.GetRotated(0f, hor, vert);
        }

        private static void f_dirToAngle(Vector3 dir, out float hor, out float vert)
        {
            Vector2 startDir2D = dir.XZ().normalized;

            float horAngle = Vector2.Angle(startDir2D, Vector2.right);
            hor = startDir2D.y > 0f ? -horAngle : horAngle;

            Vector3 floorProj = startDir2D.To_XyZ();

            float vertAngle = startDir2D.magnitude <= Vector2.kEpsilon ? 90f : Vector3.Angle(floorProj, dir);
            vert = dir.y < 0f ? -vertAngle : vertAngle;
        }
    }
}
