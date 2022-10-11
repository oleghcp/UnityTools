﻿using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityUtility.MathExt;
using static System.MathF;

namespace UnityUtility
{
    public static class MathUtility
    {
        public const float kEpsilon = FloatComparer.kEpsilon;
        public const float kEpsilonNormalSqrt = Vector3.kEpsilonNormalSqrt;
        internal const double THIRD = 1d / 3d;

        public static Vector2 Normalize(in Vector2 value, out float prevMagnitude)
        {
            prevMagnitude = value.magnitude;

            if (prevMagnitude > kEpsilon)
                return value / prevMagnitude;

            return Vector2.zero;
        }

        public static Vector3 Normalize(in Vector3 value, out float prevMagnitude)
        {
            prevMagnitude = value.magnitude;

            if (prevMagnitude > kEpsilon)
                return value / prevMagnitude;

            return Vector3.zero;
        }

        public static bool Equals(in Vector3 value, in Vector3 other, float precision)
        {
            float num1 = value.x - other.x;
            float num2 = value.y - other.y;
            float num3 = value.z - other.z;

            float result = num1 * num1 + num2 * num2 + num3 * num3;

            return result <= precision;
        }

        public static bool Equals(in Vector2 value, in Vector2 other, float precision)
        {
            float num1 = value.x - other.x;
            float num2 = value.y - other.y;

            return (num1 * num1 + num2 * num2) <= precision;
        }

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

        public static (float ch1, float ch2) LerpColorChannels(float ratio)
        {
            ratio = 1f - ratio.Clamp01();
            float ch1 = (ratio * 2f).Clamp01();
            float ch2 = (2f - ratio * 2f).Clamp01();
            return (ch1, ch2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApproachZero(float value)
        {
            return Exp(-value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApproachOne(float value)
        {
            return 1f - Exp(-value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt MinMaxRectInt(int xMin, int yMin, int xMax, int yMax)
        {
            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public static float GetCircleArea(float radius)
        {
            return PI * radius * radius;
        }

        public static float GetCircumference(float radius)
        {
            return PI * radius * 2f;
        }

        public static float GetSphereVolume(float radius)
        {
            return PI * radius * radius * radius * 4f / 3f;
        }

        public static float GetSphereSurface(float radius)
        {
            return PI * radius * radius * 4f;
        }
    }
}
