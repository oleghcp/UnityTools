using UnityEngine;
using UnityUtility.Mathematics;

namespace UnityUtility.Engine
{
    public static class UnityStructExtensions
    {
        public static bool Equals(this in Vector3 value, in Vector3 other, float precision)
        {
            return MathUtility.Equals(value, other, precision);
        }

        public static bool Equals(this in Vector2 value, in Vector2 other, float precision)
        {
            return MathUtility.Equals(value, other, precision);
        }

        public static Vector3 AlterX(this in Vector3 value, float x)
        {
            return new Vector3(x, value.y, value.z);
        }

        public static Vector3 AlterY(this in Vector3 value, float y)
        {
            return new Vector3(value.x, y, value.z);
        }

        public static Vector3 AlterZ(this in Vector3 value, float z)
        {
            return new Vector3(value.x, value.y, z);
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

        public static Vector3Int AlterX(this in Vector3Int value, int x)
        {
            return new Vector3Int(x, value.y, value.z);
        }

        public static Vector3Int AlterY(this in Vector3Int value, int y)
        {
            return new Vector3Int(value.x, y, value.z);
        }

        public static Vector3Int AlterZ(this in Vector3Int value, int z)
        {
            return new Vector3Int(value.x, value.y, z);
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

        /// <summary>
        /// Returns vector2int rotated right angles (90, 180, etc.).
        /// </summary>
        /// <param name="rotations">Defines a rotation angle by multiplying by 90 degrees. If the value is positive returns rotated counterclockwise.</param>
        public static Vector2Int GetRotated(this in Vector2Int localPos, int rotations)
        {
            (int y, int x) = MathUtility.RotateCellPos(localPos.y, localPos.x, rotations);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis.
        /// </summary>
        public static Quaternion ToEuler(this in Vector3 value)
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

        public static Vector3 Up(this in Quaternion value)
        {
            return value * Vector3.up;
        }

        public static Vector3 Down(this in Quaternion value)
        {
            return value * Vector3.down;
        }

        public static Vector3 Right(this in Quaternion value)
        {
            return value * Vector3.right;
        }

        public static Vector3 Left(this in Quaternion value)
        {
            return value * Vector3.left;
        }

        public static Vector3 Forward(this in Quaternion value)
        {
            return value * Vector3.forward;
        }

        public static Vector3 Back(this in Quaternion value)
        {
            return value * Vector3.back;
        }

        /// <summary>
        /// Returns the diagonal length of the rect.
        /// </summary>
        public static float GetDiagonal(this in Rect value)
        {
            float w = value.width;
            float h = value.height;
            return (w * w + h * h).Sqrt();
        }

        /// <summary>
        /// Returns rect expanded multiplicatively relative to zero coordinates.
        /// </summary>
        public static Rect GetMultiplied(this in Rect value, in Vector2 expandFactor)
        {
            return GetMultiplied(value, expandFactor.x, expandFactor.y);
        }

        /// <summary>
        /// Returns rect expanded multiplicatively relative to zero coordinates.
        /// </summary>
        public static Rect GetMultiplied(this in Rect value, float xFactor, float yFactor)
        {
            return Rect.MinMaxRect(value.xMin * xFactor,
                                   value.yMin * yFactor,
                                   value.xMax * xFactor,
                                   value.yMax * yFactor);
        }

        /// <summary>
        /// Returns rect expanded relative to pivot.
        /// </summary>
        /// <param name="expandPivot">Relative point of the rect from (0, 0) to (1, 1).</param>
        /// <param name="multiply">Uses multiplication for expansion if true, else uses addition.</param>
        public static Rect GetExpanded(this Rect value, in Vector2 expandSize, in Vector2 expandPivot, bool multiply = false)
        {
            Vector2 pos = value.position + Vector2.Scale(value.size, expandPivot);

            if (multiply)
                value.size = Vector2.Scale(value.size, expandSize);
            else
                value.size += expandSize;

            value.position = pos - Vector2.Scale(value.size, expandPivot);

            return value;
        }

        /// <summary>
        /// Returns additively expanded rect without preserving pivot.
        /// </summary>
        public static Rect GetExpanded(this in Rect value, in Vector2 expandSize)
        {
            return new Rect(value.position - expandSize * 0.5f, value.size + expandSize);
        }

        public static Rect ToRect(this in RectInt value)
        {
            return new Rect(value.xMin, value.yMin, value.width, value.height);
        }

        public static Rect ToRect(this in RectInt value, in Vector2 expandFactor)
        {
            return new Rect(value.xMin * expandFactor.x, value.yMin * expandFactor.y, value.width * expandFactor.x, value.height * expandFactor.y);
        }

        public static Rect ToRect(this in RectInt value, float xFactor, float yFactor)
        {
            return new Rect(value.xMin * xFactor, value.yMin * yFactor, value.width * xFactor, value.height * yFactor);
        }

        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return BitMask.HasFlag(mask, layer);
        }

        public static bool HasLayer(this LayerMask mask, string layer)
        {
            return BitMask.HasFlag(mask, LayerMask.NameToLayer(layer));
        }

        public static Color AlterR(this in Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        public static Color AlterG(this in Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }

        public static Color AlterB(this in Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }

        public static Color AlterA(this in Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        #region Deconstructors
        public static void Deconstruct(this in Vector2 vector, out float x, out float y)
        {
            x = vector.x;
            y = vector.y;
        }

        public static void Deconstruct(this in Vector3 vector, out float x, out float y, out float z)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public static void Deconstruct(this in Vector2Int vector, out int x, out int y)
        {
            x = vector.x;
            y = vector.y;
        }

        public static void Deconstruct(this in Vector3Int vector, out int x, out int y, out int z)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public static void Deconstruct(this in Color color, out float r, out float g, out float b, out float a)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public static void Deconstruct(this in Color32 color, out byte r, out byte g, out byte b, out byte a)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public static void Deconstruct(this in Rect rect, out float x, out float y, out float width, out float height)
        {
            x = rect.x;
            y = rect.y;
            width = rect.width;
            height = rect.height;
        }

        public static void Deconstruct(this in RectInt rect, out int x, out int y, out int width, out int height)
        {
            x = rect.x;
            y = rect.y;
            width = rect.width;
            height = rect.height;
        }
        #endregion
    }
}
