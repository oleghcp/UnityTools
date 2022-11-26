using UnityUtility;

namespace UnityEngine
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
#if UNITY_2019_1_OR_NEWER
            return Camera.VerticalToHorizontalFieldOfView(self.fieldOfView, (float)Screen.width / Screen.height);
#else
            return ScreenUtility.GetAspectAngle(self.fieldOfView, (float)Screen.width / Screen.height);
#endif
        }

        public static Plane[] CalculateFrustumPlanes(this Camera self)
        {
            return GeometryUtility.CalculateFrustumPlanes(self);
        }

        public static bool IsVisible(this Camera self, Renderer renderer)
        {
            return GeometryUtility.TestPlanesAABB(self.CalculateFrustumPlanes(), renderer.bounds);
        }
    }
}
