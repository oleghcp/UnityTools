using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility
{
    /// <summary>
    /// A set of common math functions.
    /// </summary>
    public static class ScreenUtility
    {
        /// <summary>
        /// Returns the current screen ratio (height/width).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetCurrentRatio()
        {
            return (float)Screen.height / Screen.width;
        }

        /// <summary>
        /// Returns scale of current screen ratio relatively to the default ratio.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRatioScale(float defaultRatio)
        {
            return defaultRatio / GetCurrentRatio();
        }

        /// <summary>
        /// Converts position form screen space to a position in UI canvas coordinates with origin at the left bottom corner.
        /// <param name="canvasHeight">Canvas rectTransform height.</param>
        /// </summary>
        public static Vector2 ScreenToUI(Vector2 screenPos, float canvasHeight)
        {
            float canvasWidth = canvasHeight / GetCurrentRatio();
            float x = canvasWidth * (screenPos.x / Screen.width);
            float y = canvasHeight * (screenPos.y / Screen.height);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Converts 2D position from world space to a position in UI canvas coordinates with origin at the left bottom corner.
        /// </summary>
        /// <param name="canvasHeight">Canvas rectTransform height.</param>
        /// <param name="camera">A camera which is used to converting.</param>
        public static Vector2 WorldToUI(Vector3 worldPos, float canvasHeight, Camera camera)
        {
            Vector2 screenPos = camera.WorldToScreenPoint(worldPos);
            return ScreenToUI(screenPos, canvasHeight);
        }

        /// <summary>
        /// Returns factor for converting screen path to world path.
        /// </summary>
        /// <param name="camSize">Camera orthographic size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetTouchScreenFactor(float camSize)
        {
            return camSize * 2f / Screen.height;
        }

        /// <summary>
        /// Returns distances from camera center to vertical and horizontal sides.
        /// </summary>
        /// <param name="camSize">Camera orthographic size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetViewRadius(float camSize)
        {
            return new Vector2(camSize / GetCurrentRatio(), camSize);
        }

        /// <summary>
        /// Calculates view bounds of the orthographic camera looking along the Z axis.
        /// </summary>
        /// <param name="camPos">Position of camera in XY-plane.</param>
        /// <param name="camSize">Camera orthographic size.</param>
        public static Rect GetViewBounds(Vector2 camPos, float camSize)
        {
            Vector2 radius = GetViewRadius(camSize);

            return Rect.MinMaxRect(camPos.x - radius.x, camPos.y - radius.y, camPos.x + radius.x, camPos.y + radius.y);
        }

        /// <summary>
        /// Calculates view bounds of the perspective camera looking along the Z axis.
        /// </summary>
        /// <param name="camPos">Position of camera in XY-plane.</param>
        /// <param name="fov">Field of view.</param>
        /// <param name="distance">Distance from the camera along Z axis.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetViewBounds(Vector2 camPos, float fov, float distance)
        {
            return GetViewBounds(camPos, distance * MathF.Tan(fov * 0.5f));
        }
    }
}
