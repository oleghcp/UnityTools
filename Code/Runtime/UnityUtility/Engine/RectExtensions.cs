using UnityEngine;
using UnityUtility.Mathematics;

namespace UnityUtility.Engine
{
    public static class RectExtensions
    {
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

        public static void Deconstruct(this in Rect value, out float x, out float y, out float width, out float height)
        {
            x = value.x;
            y = value.y;
            width = value.width;
            height = value.height;
        }

        public static void Deconstruct(this in RectInt value, out int x, out int y, out int width, out int height)
        {
            x = value.x;
            y = value.y;
            width = value.width;
            height = value.height;
        }
    }
}
