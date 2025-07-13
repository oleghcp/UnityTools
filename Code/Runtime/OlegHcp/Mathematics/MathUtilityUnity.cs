using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.Tools;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using static System.MathF;

namespace OlegHcp.Mathematics
{
    public static partial class MathUtility
    {
#pragma warning disable IDE1006
        public const float kEpsilon = FloatComparer.kEpsilon;
        public const float kEpsilonNormalSqrt = Vector3.kEpsilonNormalSqrt;
#pragma warning restore IDE1006

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
        /// Projects a vector onto another vector.
        /// </summary>
        public static Vector2 Project(in Vector2 vector, in Vector2 onNormal)
        {
            float num = Vector2.Dot(onNormal, onNormal);
            if (num > kEpsilon)
                return onNormal * Vector2.Dot(vector, onNormal) / num;

            return Vector2.zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Slerp(Vector2 a, Vector2 b, float t)
        {
            return SlerpUnclamped(a, b, t.Clamp01());
        }

        public static Vector2 SlerpUnclamped(Vector2 a, Vector2 b, float t)
        {
            float ti = 1f - t;

            float theta = Atan2(a.x * b.y - a.y * b.x, Vector2.Dot(a, b));

            float ma = Sqrt(Vector2.Dot(a, a));
            float mb = Sqrt(Vector2.Dot(b, b));

            float s = Sin(theta);
            float j = Sin(ti * theta);
            float k = Sin(t * theta);

            return (ma * ti + mb * t) * (j * mb * a + k * ma * b) / (s * ma * mb);
        }

        public static Vector3 AveragePosition(IList<Vector3> self)
        {
            if (self.Count == 0)
                throw ThrowErrors.NoElements();

            Vector3 sum = default;

            for (int i = 0; i < self.Count; i++)
            {
                sum += self[i];
            }

            return sum / self.Count;
        }

        public static Vector2 AveragePosition(IList<Vector2> self)
        {
            if (self.Count == 0)
                throw ThrowErrors.NoElements();

            Vector2 sum = default;

            for (int i = 0; i < self.Count; i++)
            {
                sum += self[i];
            }

            return sum / self.Count;
        }

        public static Vector3 AveragePosition(IEnumerable<Vector3> self)
        {
            Vector3 sum = default;

            int count = 0;
            foreach (Vector3 item in self)
            {
                sum += item;
                count++;
            }

            if (count == 0)
                throw ThrowErrors.NoElements();

            return sum / count;
        }

        public static Vector2 AveragePosition(IEnumerable<Vector2> self)
        {
            Vector2 sum = default;

            int count = 0;
            foreach (Vector2 item in self)
            {
                sum += item;
                count++;
            }

            if (count == 0)
                throw ThrowErrors.NoElements();

            return sum / count;
        }
    }
}
