﻿#if UNITY_2021_2_OR_NEWER
using System.Runtime.CompilerServices;
#endif
using UnityEngine;
using UnityUtility.MathExt;
using static System.MathF;

namespace UnityUtility
{
    public static class MathUtility
    {
        /// <summary>
        /// Rotates an array cell position.
        /// </summary>
        /// <param name="i">Row.</param>
        /// <param name="j">Column.</param>
        /// <param name="rotations">Defines a rotation angle by multiplying by 90 degrees. If the value is positive returns rotated clockwise.</param>
        public static (int i, int j) RotateCellPos(int i, int j, int rotations)
        {
            //Span<int> sinPtr = stackalloc[] { 0, 1, 0, -1 };
            //Span<int> cosPtr = stackalloc[] { 1, 0, -1, 0 };

            //rotations = rotations.Repeat(4);

            //int sin = sinPtr[rotations];
            //int cos = cosPtr[rotations];

            //int i1 = j * sin + i * cos;
            //int j1 = j * cos - i * sin;

            //return (i1, j1);

            switch (rotations.Repeat(4))
            {
                case 1: return (j, -i);
                case 2: return (-i, -j);
                case 3: return (-j, i);
                default: return (i, j);
            }
        }

        /// <summary>
        /// Rotates vector2.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public static Vector2 RotateVector(in Vector2 rotated, float angle)
        {
            angle = angle.ToRadians();
            float sin = Sin(angle);
            float cos = Cos(angle);

            return new Vector2(rotated.x * cos - rotated.y * sin, rotated.x * sin + rotated.y * cos);
        }

        /// <summary>
        /// Rotates vector3 around specified axis.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public static Vector3 RotateVector(in Vector3 rotated, in Vector3 axis, float angle)
        {
            angle = angle.ToRadians();

            float sin = Sin(angle);
            float cos = Cos(angle);

            float oneMinusCos = 1f - cos;
            float oneMinusCosByXY = oneMinusCos * axis.x * axis.y;
            float oneMinusCosByYZ = oneMinusCos * axis.y * axis.z;
            float oneMinusCosByZX = oneMinusCos * axis.z * axis.x;
            float xSin = sin * axis.x;
            float ySin = sin * axis.y;
            float zSin = sin * axis.z;

            return new Vector3
            {
                x = rotated.x * (cos + oneMinusCos * axis.x * axis.x) + rotated.y * (oneMinusCosByXY - zSin) + rotated.z * (oneMinusCosByZX + ySin),
                y = rotated.x * (oneMinusCosByXY + zSin) + rotated.y * (cos + oneMinusCos * axis.y * axis.y) + rotated.z * (oneMinusCosByYZ - xSin),
                z = rotated.x * (oneMinusCosByZX - ySin) + rotated.y * (oneMinusCosByYZ + xSin) + rotated.z * (cos + oneMinusCos * axis.z * axis.z),
            };
        }

        /// <summary>
        /// Returns vector2 corresponding to the specified angle. Default is Vector2.Right.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        public static Vector2 AngleToVector2(float angle)
        {
            angle = angle.ToRadians();
            return new Vector2(Cos(angle), Sin(angle));
        }

        /// <summary>
        /// Creates a rotation with the specified forward direction and angle around it.
        /// </summary>
        /// <param name="forward">The direction to look in.</param>
        /// <param name="angle">The angle in degrees around forward vector.</param>
        /// <returns></returns>
        public static Quaternion LookRotation(Vector3 forward, float angle)
        {
            float halfAngle = angle.ToRadians() * 0.5f;
            Vector3 xyz = forward.normalized * Sin(halfAngle);
            return new Quaternion(xyz.x, xyz.y, xyz.z, Cos(halfAngle));
        }

        public static (float ch1, float ch2) LerpColorChannels(float ratio)
        {
            ratio = 1f - ratio.Clamp01();
            float ch1 = (ratio * 2f).Clamp01();
            float ch2 = (2f - ratio * 2f).Clamp01();
            return (ch1, ch2);
        }

#if UNITY_2021_2_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clamp(long value, long min, long max)
        {
            return System.Math.Clamp(value, min, max);
#else
        public static long Clamp(long value, long min, long max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
#endif
        }

#if UNITY_2021_2_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            return System.Math.Clamp(value, min, max);
#else
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
#endif
        }
    }
}
