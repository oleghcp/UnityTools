using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.MathExt;

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
        /// Converts position form screen space to a position in UI canvas coordinates with origin at the left bottom corner.
        /// <param name="canvasHeight">Canvas rectTransform height.</param>
        /// </summary>
        public static Vector2 ScreenToUI(in Vector2 screenPos, float canvasHeight)
        {
            float canvasWidth = canvasHeight * Screen.width / Screen.height;

            return new Vector2
            {
                x = canvasWidth * (screenPos.x / Screen.width),
                y = canvasHeight * (screenPos.y / Screen.height),
            };
        }

        /// <summary>
        /// Converts 2D position from world space to a position in UI canvas coordinates with origin at the left bottom corner.
        /// </summary>
        /// <param name="canvasHeight">Canvas rectTransform height.</param>
        /// <param name="camera">A camera which is used to converting.</param>
        public static Vector2 WorldToUI(in Vector3 worldPos, float canvasHeight, Camera camera)
        {
            Vector2 screenPos = camera.WorldToScreenPoint(worldPos);
            return ScreenToUI(screenPos, canvasHeight);
        }

        /// <summary>
        /// Returns factor for converting screen path to world path.
        /// </summary>
        /// <param name="cameraOrthographicSize">Camera orthographic size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPathScreenFactor(float cameraOrthographicSize)
        {
            return cameraOrthographicSize * 2f / Screen.height;
        }

        /// <summary>
        /// Returns factor for converting screen path to world path.
        /// </summary>
        /// <param name="distance">Distance from the camera along forward axis.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetPathScreenFactor(float verticalFieldOfView, float distance)
        {
            return GetPathScreenFactor(distance * Tan(verticalFieldOfView));
        }

        /// <summary>
        /// Returns distances from orthographic camera center to vertical and horizontal sides.
        /// </summary>
        /// <param name="cameraOrthographicSize">Camera orthographic size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetCameraViewRadius(float cameraOrthographicSize)
        {
            return new Vector2(cameraOrthographicSize * Screen.width / Screen.height, cameraOrthographicSize);
        }

        /// <summary>
        /// Returns distances from point lying on perspective camera forward axis to vertical and horizontal sides.
        /// </summary>
        /// <param name="distance">Distance from the camera along forward axis.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetCameraViewRadius(float verticalFieldOfView, float distance)
        {
            return GetCameraViewRadius(distance * Tan(verticalFieldOfView));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Tan(float verticalFieldOfView)
        {
            return MathF.Tan((verticalFieldOfView * 0.5f).ToRadians());
        }
    }
}
