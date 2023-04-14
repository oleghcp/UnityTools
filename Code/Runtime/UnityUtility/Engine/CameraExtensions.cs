using UnityEngine;

namespace UnityUtility.Engine
{
    public static class CameraExtensions
    {
        public static Vector2 GetOrthographicSize(this Camera self)
        {
            return ScreenUtility.GetOrthographicSize(self.orthographicSize);
        }

        public static Vector2 GetPerspectiveSize(this Camera self, float remoteness)
        {
            return ScreenUtility.GetPerspectiveSize(self.fieldOfView, remoteness);
        }

        public static float GetHorizontalFov(this Camera self)
        {
            return Camera.VerticalToHorizontalFieldOfView(self.fieldOfView, (float)Screen.width / Screen.height);
        }

        public static Plane[] CalculateFrustumPlanes(this Camera self)
        {
            return GeometryUtility.CalculateFrustumPlanes(self);
        }

        public static bool CanSee(this Camera self, Renderer target)
        {
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(self), target.bounds);
        }
    }
}
