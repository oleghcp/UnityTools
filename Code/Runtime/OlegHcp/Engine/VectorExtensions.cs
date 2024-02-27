using System;
using System.Collections.Generic;
using UnityEngine;
using OlegHcp.Mathematics;

namespace OlegHcp.Engine
{
    public static class VectorExtensions
    {
        public static Vector3 IncreaseX(this Vector3 value, float xIncrease)
        {
            value.x += xIncrease;
            return value;
        }

        public static Vector2 IncreaseX(this Vector2 value, float xIncrease)
        {
            value.x += xIncrease;
            return value;
        }

        public static Vector3 IncreaseY(this Vector3 value, float yIncrease)
        {
            value.y += yIncrease;
            return value;
        }

        public static Vector2 IncreaseY(this Vector2 value, float yIncrease)
        {
            value.y += yIncrease;
            return value;
        }

        public static Vector3 IncreaseZ(this Vector3 value, float zIncrease)
        {
            value.z += zIncrease;
            return value;
        }

        public static Vector3 DecreaseX(this Vector3 value, float xDecrease)
        {
            value.x -= xDecrease;
            return value;
        }

        public static Vector2 DecreaseX(this Vector2 value, float xDecrease)
        {
            value.x -= xDecrease;
            return value;
        }

        public static Vector3 DecreaseY(this Vector3 value, float yDecrease)
        {
            value.y -= yDecrease;
            return value;
        }

        public static Vector2 DecreaseY(this Vector2 value, float yDecrease)
        {
            value.y -= yDecrease;
            return value;
        }

        public static Vector3 DecreaseZ(this Vector3 value, float zDecrease)
        {
            value.z -= zDecrease;
            return value;
        }

        public static Vector3 MultiplyX(this Vector3 value, float xMultiply)
        {
            value.x *= xMultiply;
            return value;
        }

        public static Vector2 MultiplyX(this Vector2 value, float xMultiply)
        {
            value.x *= xMultiply;
            return value;
        }

        public static Vector3 MultiplyY(this Vector3 value, float yMultiply)
        {
            value.y *= yMultiply;
            return value;
        }

        public static Vector2 MultiplyY(this Vector2 value, float yMultiply)
        {
            value.y *= yMultiply;
            return value;
        }

        public static Vector3 MultiplyZ(this Vector3 value, float zMultiply)
        {
            value.z *= zMultiply;
            return value;
        }

        public static Vector3 DivideX(this Vector3 value, float xDivide)
        {
            value.x /= xDivide;
            return value;
        }

        public static Vector2 DivideX(this Vector2 value, float xDivide)
        {
            value.x /= xDivide;
            return value;
        }

        public static Vector3 DivideY(this Vector3 value, float yDivide)
        {
            value.y /= yDivide;
            return value;
        }

        public static Vector2 DivideY(this Vector2 value, float yDivide)
        {
            value.y /= yDivide;
            return value;
        }

        public static Vector3 DivideZ(this Vector3 value, float zDivide)
        {
            value.z /= zDivide;
            return value;
        }

        public static Vector3 Abs(this in Vector3 value)
        {
            return new Vector3(Math.Abs(value.x), Math.Abs(value.y), Math.Abs(value.z));
        }

        public static Vector2 Abs(this in Vector2 value)
        {
            return new Vector2(Math.Abs(value.x), Math.Abs(value.y));
        }

        public static Vector3 Invert(this in Vector3 value)
        {
            return new Vector3(-value.x, -value.y, -value.z);
        }

        public static Vector2 Invert(this in Vector2 value)
        {
            return new Vector2(-value.x, -value.y);
        }

        public static bool Equals(this in Vector3 value, in Vector3 other, float precision)
        {
            return MathUtility.Equals(value, other, precision);
        }

        public static bool Equals(this in Vector2 value, in Vector2 other, float precision)
        {
            return MathUtility.Equals(value, other, precision);
        }

        public static Vector3 AlterX(this Vector3 value, float x)
        {
            value.x = x;
            return value;
        }

        public static Vector2 AlterX(this Vector2 value, float x)
        {
            value.x = x;
            return value;
        }

        public static Vector3 AlterY(this Vector3 value, float y)
        {
            value.y = y;
            return value;
        }

        public static Vector2 AlterY(this Vector2 value, float y)
        {
            value.y = y;
            return value;
        }

        public static Vector3 AlterZ(this Vector3 value, float z)
        {
            value.z = z;
            return value;
        }

        public static Vector3Int AlterX(this Vector3Int value, int x)
        {
            value.x = x;
            return value;
        }

        public static Vector2Int AlterX(this Vector2Int value, int x)
        {
            value.x = x;
            return value;
        }

        public static Vector3Int AlterY(this Vector3Int value, int y)
        {
            value.y = y;
            return value;
        }

        public static Vector2Int AlterY(this Vector2Int value, int y)
        {
            value.y = y;
            return value;
        }

        public static Vector3Int AlterZ(this Vector3Int value, int z)
        {
            value.z = z;
            return value;
        }

        /// <summary>
        /// Creates vector2 based on X and Y of vector3 value.
        /// </summary>
        public static Vector2 XY(this in Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }

        /// <summary>
        /// Creates vector2 based on X and Y of vector3 value.
        /// </summary>
        public static Vector2 XY(this in Vector3 value, out float z)
        {
            z = value.z;
            return new Vector2(value.x, value.y);
        }

        /// <summary>
        /// Creates vector2 based on X and Z of vector3 value.
        /// </summary>
        public static Vector2 XZ(this in Vector3 value)
        {
            return new Vector2(value.x, value.z);
        }

        /// <summary>
        /// Creates vector2 based on X and Z of vector3 value.
        /// </summary>
        public static Vector2 XZ(this in Vector3 value, out float y)
        {
            y = value.y;
            return new Vector2(value.x, value.z);
        }

        /// <summary>
        /// Creates vector2 based on Y and Z of vector3 value.
        /// </summary>
        public static Vector2 YZ(this in Vector3 value)
        {
            return new Vector2(value.z, value.y);
        }

        /// <summary>
        /// Creates vector2 based on Y and Z of vector3 value.
        /// </summary>
        public static Vector2 YZ(this in Vector3 value, out float x)
        {
            x = value.x;
            return new Vector2(value.z, value.y);
        }

        /// <summary>
        /// Converts vector2 params to vector3 X and Y with custom Z
        /// </summary>
        public static Vector3 To_XYz(this in Vector2 value, float z = 0f)
        {
            return new Vector3(value.x, value.y, z);
        }

        /// <summary>
        /// Converts vector2 params to vector3 X and Z with custom Y
        /// </summary>
        public static Vector3 To_XyZ(this in Vector2 value, float y = 0f)
        {
            return new Vector3(value.x, y, value.y);
        }

        /// <summary>
        /// Converts vector2 params to vector3 Y and Z with custom X
        /// </summary>
        public static Vector3 To_xYZ(this in Vector2 value, float x = 0f)
        {
            return new Vector3(x, value.y, value.x);
        }

        /// <summary>
        /// Creates vector2 based on X and Y of vector3 value.
        /// </summary>
        public static Vector2Int XY(this in Vector3Int value)
        {
            return new Vector2Int(value.x, value.y);
        }

        /// <summary>
        /// Creates vector2 based on X and Z of vector3 value.
        /// </summary>
        public static Vector2Int XZ(this in Vector3Int value)
        {
            return new Vector2Int(value.x, value.z);
        }

        /// <summary>
        /// Creates vector2 based on Y and Z of vector3 value.
        /// </summary>
        public static Vector2Int YZ(this in Vector3Int value)
        {
            return new Vector2Int(value.z, value.y);
        }

        /// <summary>
        /// Converts vector2 params to vector3 X and Y with custom Z
        /// </summary>
        public static Vector3Int To_XYz(this in Vector2Int value, int z = 0)
        {
            return new Vector3Int(value.x, value.y, z);
        }

        /// <summary>
        /// Converts vector2 params to vector3 X and Z with custom Y
        /// </summary>
        public static Vector3Int To_XyZ(this in Vector2Int value, int y = 0)
        {
            return new Vector3Int(value.x, y, value.y);
        }

        /// <summary>
        /// Converts vector2 params to vector3 Y and Z with custom X
        /// </summary>
        public static Vector3Int To_xYZ(this in Vector2Int value, int x = 0)
        {
            return new Vector3Int(x, value.y, value.x);
        }

        public static Vector2 GetNormalized(this in Vector2 value, out float prevMagnitude)
        {
            return MathUtility.Normalize(value, out prevMagnitude);
        }

        public static Vector3 GetNormalized(this in Vector3 value, out float prevMagnitude)
        {
            return MathUtility.Normalize(value, out prevMagnitude);
        }

        /// <summary>
        /// Projects a vector onto another vector.
        /// </summary>
        public static Vector2 Project(this in Vector2 self, in Vector2 onNormal)
        {
            return MathUtility.Project(self, onNormal);
        }

        /// <summary>
        /// Projects a vector onto another vector.
        /// </summary>
        public static Vector3 Project(this in Vector3 self, in Vector3 onNormal)
        {
            return Vector3.Project(self, onNormal);
        }

        /// <summary>
        /// Projects a vector onto a plane defined by a normal orthogonal to the plane.
        /// </summary>
        public static Vector3 ProjectOnPlane(this in Vector3 self, in Vector3 planeNormal)
        {
            return Vector3.ProjectOnPlane(self, planeNormal);
        }

        /// <summary>
        /// Returns vector2 value clamped between values represented by the bounds.
        /// </summary>
        public static Vector2Int GetClamped(this in Vector2Int value, in RectInt bounds)
        {
            return new Vector2Int
            {
                x = value.x.Clamp(bounds.xMin, bounds.xMax),
                y = value.y.Clamp(bounds.yMin, bounds.yMax)
            };
        }

        /// <summary>
        /// Returns vector2 position clamped in bounds.
        /// </summary>
        public static Vector2 GetClamped(this in Vector2 value, in Rect bounds)
        {
            return new Vector2
            {
                x = value.x.Clamp(bounds.xMin, bounds.xMax),
                y = value.y.Clamp(bounds.yMin, bounds.yMax),
            };
        }

        /// <summary>
        /// Returns a copy of vector with its magnitude clamped to maxLength.
        /// </summary>
        public static Vector2 GetClamped(this in Vector2 value, float maxLength)
        {
            return Vector2.ClampMagnitude(value, maxLength);
        }

        /// <summary>
        /// Returns vector2 position clamped in bounds.
        /// </summary>
        public static Vector2 GetClamped(this in Vector3 value, in Bounds bounds)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            return new Vector3
            {
                x = value.x.Clamp(min.x, max.x),
                y = value.y.Clamp(min.y, max.y),
                z = value.z.Clamp(min.z, max.z),
            };
        }

        /// <summary>
        /// Returns a copy of vector with its magnitude clamped to maxLength.
        /// </summary>
        public static Vector3 GetClamped(this in Vector3 value, float maxLength)
        {
            return Vector3.ClampMagnitude(value, maxLength);
        }

        /// <summary>
        /// Returns vector2 rotated in XY-plane.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public static Vector2 GetRotated(this in Vector2 value, float angle)
        {
            return MathUtility.RotateVector(value, angle);
        }

        /// <summary>
        /// Returns vector3 rotated around specified axis.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public static Vector3 GetRotated(this in Vector3 value, in Vector3 axis, float angle)
        {
            return MathUtility.RotateVector(value, axis, angle);
        }

        public static Vector3 GetRotated(this in Vector3 value, in Quaternion quaternion)
        {
            return quaternion * value;
        }

        /// <summary>
        /// Returns vector2int rotated right angles (90, 180, etc.).
        /// </summary>
        /// <param name="rotations">Defines a rotation angle by multiplying by 90 degrees. If the value is positive returns rotated counterclockwise.</param>
        public static Vector2Int GetRotated(this in Vector2Int localPos, int rotations)
        {
            (int y, int x) = MathUtility.RotateCellPos(localPos.y, localPos.x, rotations);
            return new Vector2Int(x, y);
        }

        public static Vector3 GetReflected(this in Vector3 value, in Vector3 normal)
        {
            return Vector3.Reflect(value, normal);
        }

        /// <summary>
        /// Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis.
        /// </summary>
        public static Quaternion ToEulerRotation(this in Vector3 value)
        {
            return Quaternion.Euler(value);
        }

        /// <summary>
        /// Creates a rotation with the specified forward and upwards directions.
        /// </summary>
        public static Quaternion ToLookRotation(this in Vector3 value, Vector3 upwards)
        {
            return Quaternion.LookRotation(value, upwards);
        }

        /// <summary>
        /// Creates a rotation with the specified forward direction and angle around it.
        /// </summary>
        public static Quaternion ToLookRotation(this in Vector3 value)
        {
            return Quaternion.LookRotation(value);
        }

        public static Vector3 AveragePosition(this IList<Vector3> self)
        {
            return MathUtility.AveragePosition(self);
        }

        public static Vector2 AveragePosition(this IList<Vector2> self)
        {
            return MathUtility.AveragePosition(self);
        }

        public static Vector3 AveragePosition(this IEnumerable<Vector3> self)
        {
            return MathUtility.AveragePosition(self);
        }

        public static Vector2 AveragePosition(this IEnumerable<Vector2> self)
        {
            return MathUtility.AveragePosition(self);
        }

        #region Deconstruction
        public static void Deconstruct(this in Vector2 value, out float x, out float y)
        {
            x = value.x;
            y = value.y;
        }

        public static void Deconstruct(this in Vector3 value, out float x, out float y, out float z)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }

        public static void Deconstruct(this in Vector2Int value, out int x, out int y)
        {
            x = value.x;
            y = value.y;
        }

        public static void Deconstruct(this in Vector3Int value, out int x, out int y, out int z)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
        #endregion
    }
}
