using System.Runtime.CompilerServices;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityEngine
{
    public static class UnityStructExtensions
    {
        public static Vector3 AlterX(this Vector3 value, float x = 0f)
        {
            value.x = x;
            return value;
        }

        public static Vector3 AlterY(this Vector3 value, float y = 0f)
        {
            value.y = y;
            return value;
        }

        public static Vector3 AlterZ(this Vector3 value, float z = 0f)
        {
            value.z = z;
            return value;
        }

        //--//

        /// <summary>
        /// Creates vector2 based on X and Y of vector3 value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XY(this in Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }

        /// <summary>
        /// Creates vector2 based on X and Z of vector3 value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XZ(this in Vector3 value)
        {
            return new Vector2(value.x, value.z);
        }

        /// <summary>
        /// Creates vector2 based on Y and Z of vector3 value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YZ(this in Vector3 value)
        {
            return new Vector2(value.z, value.y);
        }

        //--//

        /// <summary>
        /// Converts vector2 params to vector3 X and Y with custom Z
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 To_XYz(this in Vector2 value, float z = 0f)
        {
            return new Vector3(value.x, value.y, z);
        }

        /// <summary>
        /// Converts vector2 params to vector3 X and Z with custom Y
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 To_XyZ(this in Vector2 value, float y = 0f)
        {
            return new Vector3(value.x, y, value.y);
        }

        /// <summary>
        /// Converts vector2 params to vector3 Y and Z with custom X
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 To_xYZ(this in Vector2 value, float x = 0f)
        {
            return new Vector3(x, value.y, value.x);
        }

        //--//

        public static Vector3Int AlterX(this Vector3Int value, int x = 0)
        {
            value.x = x;
            return value;
        }

        public static Vector3Int AlterY(this Vector3Int value, int y = 0)
        {
            value.y = y;
            return value;
        }

        public static Vector3Int AlterZ(this Vector3Int value, int z = 0)
        {
            value.z = z;
            return value;
        }

        //--//

        /// <summary>
        /// Creates vector2 based on X and Y of vector3 value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int XY(this in Vector3Int value)
        {
            return new Vector2Int(value.x, value.y);
        }

        /// <summary>
        /// Creates vector2 based on X and Z of vector3 value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int XZ(this in Vector3Int value)
        {
            return new Vector2Int(value.x, value.z);
        }

        /// <summary>
        /// Creates vector2 based on Y and Z of vector3 value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int YZ(this in Vector3Int value)
        {
            return new Vector2Int(value.z, value.y);
        }

        //--//

        /// <summary>
        /// Converts vector2 params to vector3 X and Y with custom Z
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int To_XYz(this in Vector2Int value, int z = 0)
        {
            return new Vector3Int(value.x, value.y, z);
        }

        /// <summary>
        /// Converts vector2 params to vector3 X and Z with custom Y
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int To_XyZ(this in Vector2Int value, int y = 0)
        {
            return new Vector3Int(value.x, y, value.y);
        }

        /// <summary>
        /// Converts vector2 params to vector3 Y and Z with custom X
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int To_xYZ(this in Vector2Int value, int x = 0)
        {
            return new Vector3Int(x, value.y, value.x);
        }

        /// <summary>
        /// Is position in bounds.
        /// </summary>
        /// <param name="bounds">Checked bounds</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InBounds(this in Vector2Int pos, in RectInt bounds)
        {
            return bounds.Contains(pos);
        }

        /// <summary>
        /// Is position in bounds.
        /// </summary>
        /// <param name="bounds">Checked bounds</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InBounds(this in Vector3Int pos, in RectInt bounds)
        {
            return bounds.Contains(pos.XY());
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

        //--//

        /// <summary>
        /// Returns vector2 rotated in XY-plane.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetRotated(this in Vector2 value, float angle)
        {
            return MathUtility.RotateVector(value, angle);
        }

        /// <summary>
        /// Returns vector3 rotated in space.
        /// </summary>
        /// <param name="euler">Euler engles for rotation in degrees.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRotated(this in Vector3 value, in Vector3 euler)
        {
            return Quaternion.Euler(euler) * value;
        }

        /// <summary>
        /// Returns vector3 rotated in space.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRotated(this in Vector3 value, float xAngle, float yAngle, float zAngle)
        {
            return Quaternion.Euler(xAngle, yAngle, zAngle) * value;
        }

        /// <summary>
        /// Returns vector3 rotated around specified axis.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRotated(this in Vector3 value, in Vector3 axis, float angle)
        {
            return MathUtility.RotateVector(value, axis, angle);
        }

        /// <summary>
        /// Returns vector2int rotated right angles (90, 180, etc.).
        /// </summary>
        /// <param name="rotations">Defines a rotation angle by multiplying by 90 degrees. If the value is positive returns rotated counterclockwise.</param>
        public static Vector2Int GetRotated(this Vector2Int localPos, int rotations)
        {
            (int y, int x) = MathUtility.RotateCellPos(localPos.y, localPos.x, rotations);

            localPos.x = x;
            localPos.y = y;

            return localPos;
        }

        //--//

        /// <summary>
        /// Returns vector2 value clamped between values represented by the bounds.
        /// </summary>
        public static Vector2 GetClamped(this in Vector2 value, in Rect bounds)
        {
            return new Vector2
            {
                x = value.x.Clamp(bounds.xMin, bounds.xMax),
                y = value.y.Clamp(bounds.yMin, bounds.yMax)
            };
        }

        /// <summary>
        /// Returns a copy of vector with its magnitude clamped to maxLength.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetClamped(this in Vector2 value, float maxLength)
        {
            return Vector2.ClampMagnitude(value, maxLength);
        }

        /// <summary>
        /// Returns a copy of vector with its magnitude clamped to maxLength.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetClamped(this in Vector3 value, float maxLength)
        {
            return Vector3.ClampMagnitude(value, maxLength);
        }

        /// <summary>
        /// Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ToRotation(this in Vector3 value)
        {
            return Quaternion.Euler(value);
        }

        /// <summary>
        /// Is position in bounds.
        /// </summary>
        /// <param name="bounds">Checked bounds</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InBounds(this in Vector2 pos, in Rect bounds)
        {
            return bounds.Contains(pos);
        }

        /// <summary>
        /// Is position in bounds.
        /// </summary>
        /// <param name="bounds">Checked bounds</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InBounds(this in Vector3 pos, in Rect bounds)
        {
            return bounds.Contains(pos);
        }

        /// <summary>
        /// Returns the aspect ratio of the rect (height / width).
        /// </summary>
        public static float GetAspectRatio(this in Rect value)
        {
            return value.height / value.width;
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
        /// Returns pivot in zero coordinates relative to the rect.
        /// </summary>
        public static Vector2 GetPivot(this in Rect rect)
        {
            float x = (0f - rect.xMin) / rect.width;
            float y = (0f - rect.yMin) / rect.height;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Returns rect expanded multiplicatively relative to zero coordinates.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetMultiplied(this in Rect value, in Vector2 expandFactor)
        {
            return GetMultiplied(value, expandFactor.x, expandFactor.y);
        }

        /// <summary>
        /// Returns rect expanded multiplicatively relative to zero coordinates.
        /// </summary>
        public static Rect GetMultiplied(this Rect value, float xFactor, float yFactor)
        {
            value.xMin *= xFactor;
            value.yMin *= yFactor;

            value.xMax *= xFactor;
            value.yMax *= yFactor;

            return value;
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
        public static Rect GetExpanded(this Rect value, in Vector2 expandSize)
        {
            value.size += expandSize;
            value.position -= expandSize * 0.5f;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this in RectInt value)
        {
            return new Rect(value.xMin, value.yMin, value.width, value.height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this in RectInt value, in Vector2 expandFactor)
        {
            return new Rect(value.xMin * expandFactor.x, value.yMin * expandFactor.y, value.width * expandFactor.x, value.height * expandFactor.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ToRect(this in RectInt value, float xFactor, float yFactor)
        {
            return new Rect(value.xMin * xFactor, value.yMin * yFactor, value.width * xFactor, value.height * yFactor);
        }

        //--//

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return (mask & 1 << layer) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasLayer(this LayerMask mask, string layer)
        {
            return (mask & 1 << LayerMask.NameToLayer(layer)) != 0;
        }
    }
}
