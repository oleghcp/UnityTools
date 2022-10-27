using System;
using System.Collections.Generic;
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
        /// Returns existing component or adds and returns new one.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
#if UNITY_2019_2_OR_NEWER
            if (self.TryGetComponent(out T component))
#else
            T component = self.GetComponent<T>();
            if (component != null)
#endif
                return component;

            return self.AddComponent<T>();
        }

        /// <summary>
        /// Returns existing component of Type type or adds and returns new one.
        /// </summary>
        public static Component GetOrAddComponent(this GameObject self, Type type)
        {
#if UNITY_2019_2_OR_NEWER
            if (self.TryGetComponent(type, out Component component))
#else
            Component component = self.GetComponent(type);
            if (component != null)
#endif
                return component;

            return self.AddComponent(type);
        }

        /// <summary>
        /// Marks unity object as DontDestroyOnLoad.
        /// </summary>
        public static void Immortalize(this UnityObject self)
        {
            UnityObject.DontDestroyOnLoad(self);
        }

        public static void SetParent(this GameObject self, GameObject parent)
        {
            self.transform.SetParent(parent.transform);
        }

        public static void SetParent(this GameObject self, Transform parent)
        {
            self.transform.SetParent(parent);
        }

        /// <summary>
        /// Returns an ancestor from the transform's hierarchy on the specified hierarchy level.
        /// </summary>
        public static GameObject GetParent(this GameObject self, int level = 1)
        {
            Transform parent = self.transform.GetParent(level);

            if (parent == null)
                return null;

            return parent.gameObject;
        }

        /// <summary>
        /// Returns IEnumerable collection of all children;
        /// </summary>
        public static IEnumerable<GameObject> EnumerateChildren(this GameObject self, bool recursive)
        {
            Transform root = self.transform;
            int count = root.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform child = root.GetChild(i);

                yield return child.gameObject;

                if (recursive)
                {
                    foreach (Transform subChild in child.EnumerateChildren(true))
                    {
                        yield return subChild.gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// Returns children of the top level of the hierarchy.
        /// </summary>
        public static GameObject[] GetTopChildren(this GameObject self)
        {
            Transform root = self.transform;
            int count = root.childCount;

            GameObject[] children = new GameObject[count];

            for (int i = 0; i < count; i++)
            {
                children[i] = root.GetChild(i).gameObject;
            }

            return children;
        }

        /// <summary>
        /// Finds all children.
        /// </summary>
        public static List<GameObject> GetAllChildren(this GameObject self)
        {
            List<GameObject> list = new List<GameObject>();

            GetAllChildren(self, list);

            return list;
        }

        /// <summary>
        /// Finds all children.
        /// </summary>
        public static void GetAllChildren(this GameObject self, List<GameObject> list)
        {
            Transform root = self.transform;
            int count = root.childCount;

            for (int i = 0; i < count; i++)
            {
                GameObject ch = root.GetChild(i).gameObject;

                list.Add(ch);

                GetAllChildren(ch, list);
            }
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

        /// <summary>
        /// Returns transform of gameobject as RectTransform if posible. Otherwise returns null.
        /// </summary>
        public static RectTransform GetRectTransform(this GameObject self)
        {
            return self.transform as RectTransform;
        }

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
