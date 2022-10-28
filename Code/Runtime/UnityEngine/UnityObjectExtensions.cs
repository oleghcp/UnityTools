using UnityUtility;
using UnityObject = UnityEngine.Object;

namespace UnityEngine
{
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Destroys the unity object.
        /// </summary>
        /// <param name="time">Time  for destruction.</param>
        public static void Destroy(this UnityObject self, float time)
        {
            UnityObject.Destroy(self, time);
        }

        /// <summary>
        /// Destroys the unity object.
        /// </summary>
        public static void Destroy(this UnityObject self)
        {
            UnityObject.Destroy(self, 0f);
        }

        /// <summary>
        /// Marks unity object as DontDestroyOnLoad.
        /// </summary>
        public static void Immortalize(this UnityObject self)
        {
            UnityObject.DontDestroyOnLoad(self);
        }

#if !UNITY_2021_2_OR_NEWER
        public static T GetComponentInParent<T>(this Component self, bool includeInactive)
        {
            return self.transform.GetComponentInParent<T>(includeInactive);
        }

        public static T GetComponentInParent<T>(this GameObject self, bool includeInactive)
        {
            return self.transform.GetComponentInParent<T>(includeInactive);
        }

        public static T GetComponentInParent<T>(this Transform self, bool includeInactive)
        {
            if (!includeInactive)
                return self.GetComponentInParent<T>();

            for (Transform p = self; p != null; p = p.parent)
            {
                T component = p.GetComponent<T>();
                if (component != null)
                    return component;
            }

            return default;
        }

        public static T GetComponentInChildren<T>(this Component self, bool includeInactive)
        {
            return self.transform.GetComponentInChildren<T>(includeInactive);
        }

        public static T GetComponentInChildren<T>(this GameObject self, bool includeInactive)
        {
            return self.transform.GetComponentInChildren<T>(includeInactive);
        }

        public static T GetComponentInChildren<T>(this Transform self, bool includeInactive)
        {
            if (!includeInactive)
                return self.GetComponentInChildren<T>();

            foreach (Transform child in self.EnumerateChildren(true))
            {
                T component = child.GetComponent<T>();
                if (component != null)
                    return component;
            }

            return default;
        }
#endif
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

        /// <summary>
        /// Creates a sprite.
        /// </summary>
        public static Sprite ToSprite(this Texture2D self, in Vector2 pivot, float pixelsPerUnit = 100f, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.Tight, in Vector4 border = default)
        {
            return Sprite.Create(self, new Rect(0f, 0f, self.width, self.height), pivot, pixelsPerUnit, extrude, meshType, border);
        }

        /// <summary>
        /// Creates a sprite with the pivot in the center.
        /// </summary>
        public static Sprite ToSprite(this Texture2D self, float pixelsPerUnit = 100f, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.Tight, in Vector4 border = default)
        {
            float x = self.width;
            float y = self.height;
            return Sprite.Create(self, new Rect(0f, 0f, x, y), new Vector2(x * 0.5f, y * 0.5f), pixelsPerUnit, extrude, meshType, border);
        }

        public static bool IsNullOrDead(this UnityObject self)
        {
            return UnityObjectUtility.IsNullOrDead(self);
        }

        public static bool IsAlive(this UnityObject self)
        {
            return !UnityObjectUtility.IsNullOrDead(self);
        }

        /// <summary>
        /// Returns true if a game object is asset reference. For scene objects returns false.
        /// </summary>
        public static bool IsAsset(this GameObject self)
        {
            return UnityObjectUtility.IsAsset(self);
        }

        /// <summary>
        /// Returns true if a Component is asset reference. For scene objects returns false.
        /// </summary>
        public static bool IsAsset(this Component self)
        {
            return UnityObjectUtility.IsAsset(self);
        }

        /// <summary>
        /// Returns true if UnityEngine.Object is asset reference.
        /// </summary>
        public static bool IsAsset(this UnityObject self)
        {
            return UnityObjectUtility.IsAsset(self);
        }
    }
}
