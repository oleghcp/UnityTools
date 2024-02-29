using System.Runtime.CompilerServices;
using UnityEngine;

namespace OlegHcp
{
    public enum RectTransformStretch
    {
        Left,
        Right,
        Top,
        Bottom,
        MiddleHorizontal,
        MiddleVertical,
        Full,
    }

    public static class RectUtility
    {
        public static Vector2 GetAnchor(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return new Vector2(0f, 1f);
                case TextAnchor.UpperCenter: return new Vector2(0.5f, 1f);
                case TextAnchor.UpperRight: return new Vector2(1f, 1f);
                case TextAnchor.MiddleLeft: return new Vector2(0f, 0.5f);
                case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight: return new Vector2(1f, 0.5f);
                case TextAnchor.LowerLeft: return new Vector2(0f, 0f);
                case TextAnchor.LowerCenter: return new Vector2(0.5f, 0f);
                case TextAnchor.LowerRight: return new Vector2(1f, 0f);
                default: throw new UnsupportedValueException(anchor);
            }
        }

        public static Rect GetAnchor(RectTransformStretch stretch, out Vector2 pivot)
        {
            switch (stretch)
            {
                case RectTransformStretch.Left:
                    pivot = new Vector2(0f, 0.5f);
                    return Rect.MinMaxRect(0f, 0f, 0f, 1f);

                case RectTransformStretch.Right:
                    pivot = new Vector2(1f, 0.5f);
                    return Rect.MinMaxRect(1f, 0f, 1f, 1f);

                case RectTransformStretch.Top:
                    pivot = new Vector2(0.5f, 1f);
                    return Rect.MinMaxRect(0f, 1f, 1f, 1f);

                case RectTransformStretch.Bottom:
                    pivot = new Vector2(0.5f, 0f);
                    return Rect.MinMaxRect(0f, 0f, 1f, 0f);

                case RectTransformStretch.MiddleHorizontal:
                    pivot = new Vector2(0.5f, 0.5f);
                    return Rect.MinMaxRect(0f, 0.5f, 1f, 0.5f);

                case RectTransformStretch.MiddleVertical:
                    pivot = new Vector2(0.5f, 0.5f);
                    return Rect.MinMaxRect(0.5f, 0f, 0.5f, 1f);

                case RectTransformStretch.Full:
                    pivot = new Vector2(0.5f, 0.5f);
                    return Rect.MinMaxRect(0f, 0f, 1f, 1f);

                default: throw new UnsupportedValueException(stretch);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt MinMaxRectInt(int xMin, int yMin, int xMax, int yMax)
        {
            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}
