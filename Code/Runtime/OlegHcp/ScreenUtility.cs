using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using OlegHcp.Mathematics;

namespace OlegHcp
{
    /// <summary>
    /// A set of common math functions.
    /// </summary>
    public static class ScreenUtility
    {
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
            return GetPathScreenFactor(distance * GetHalfFovTan(verticalFieldOfView));
        }

        /// <summary>
        /// Returns distances from orthographic camera center to vertical and horizontal sides.
        /// </summary>
        /// <param name="cameraOrthographicSize">Camera orthographic size.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetOrthographicSize(float cameraOrthographicSize)
        {
            return new Vector2(cameraOrthographicSize * Screen.width / Screen.height, cameraOrthographicSize);
        }

        /// <summary>
        /// Returns distances from point lying on perspective camera forward axis to vertical and horizontal sides.
        /// </summary>
        /// <param name="remoteness">Distance from the camera along forward axis.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPerspectiveSize(float verticalFieldOfView, float remoteness)
        {
            return GetOrthographicSize(remoteness * GetHalfFovTan(verticalFieldOfView));
        }

        /// <summary>
        /// Returns horizontal field of view for known vertical one and vice versa.
        /// </summary>        
        /// <param name="aspectRatio">Value is width/height for converting vertical fov to horizontal one and vice versa.</param>
        public static float GetAspectAngle(float fieldOfView, float aspectRatio)
        {
            float halfTan = GetHalfFovTan(fieldOfView) * aspectRatio;
            return GetFovFromHalfTan(halfTan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float GetHalfFovTan(float fov)
        {
            return MathF.Tan((fov * 0.5f).ToRadians());
        }

        internal static float GetFovFromHalfTan(float halfFovTan)
        {
            return MathF.Atan(halfFovTan).ToDegrees() * 2f;
        }
    }
}
