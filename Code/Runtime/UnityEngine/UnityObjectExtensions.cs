using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(this UnityObject self, float time)
        {
            UnityObject.Destroy(self, time);
        }

        /// <summary>
        /// Destroys the unity object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(this UnityObject self)
        {
            UnityObject.Destroy(self, 0f);
        }

        /// <summary>
        /// Instantiates unity object of defined type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Install<T>(this T self) where T : UnityObject
        {
            return UnityObject.Instantiate(self);
        }

        /// <summary>
        /// Instantiates unity object of defined type.
        /// </summary>
        public static T Install<T>(this T self, HideFlags hideFlags) where T : UnityObject
        {
            T obj = UnityObject.Instantiate(self);
            obj.hideFlags = hideFlags;
            return obj;
        }

        /// <summary>
        /// Instantiates gameobject as a child of the specified parent.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Install(this GameObject self, Transform parent, bool worldPositionStays)
        {
            return UnityObject.Instantiate(self, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates gameobject as a child with default local position and rotation.
        /// </summary>
        public static GameObject Install(this GameObject self, Transform parent)
        {
            return UnityObject.Instantiate(self, parent, false);
        }

        /// <summary>
        /// Instantiates gameobject as a child of the specified parent.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Install<T>(this T self, Transform parent, bool worldPositionStays) where T : Component
        {
            return UnityObject.Instantiate(self, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates defined component as a child with default local position and rotation.
        /// </summary>
        public static T Install<T>(this T self, Transform parent) where T : Component
        {
            return UnityObject.Instantiate(self, parent, false);
        }

        /// <summary>
        /// Instantiates gameobject to the specified position with the specified rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Install(this GameObject self, in Vector3 position)
        {
            return UnityObject.Instantiate(self, position, Quaternion.identity);
        }

        /// <summary>
        /// Instantiates gameobject to the specified position with the specified rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Install(this GameObject self, in Vector3 position, in Quaternion rotation)
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates gameobject as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static GameObject Install(this GameObject self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local = true)
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Instantiates defined component to specified position with specified rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Install<T>(this T self, in Vector3 position) where T : Component
        {
            return UnityObject.Instantiate(self, position, Quaternion.identity);
        }

        /// <summary>
        /// Instantiates defined component to specified position with specified rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Install<T>(this T self, in Vector3 position, in Quaternion rotation) where T : Component
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates gameobject as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static T Install<T>(this T self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local = true) where T : Component
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Returns existing component or adds and returns new one.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var component = self.GetComponent<T>();
            if (component == null)
                return self.AddComponent<T>();
            return component;
        }

        /// <summary>
        /// Returns existing component of Type type or adds and returns new one.
        /// </summary>
        public static Component GetOrAddComponent(this GameObject self, Type type)
        {
            var component = self.GetComponent(type);
            if (component == null)
                return self.AddComponent(type);
            return component;
        }

        /// <summary>
        /// Marks unity object as DontDestroyOnLoad.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Immortalize(this UnityObject self)
        {
            UnityObject.DontDestroyOnLoad(self);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this GameObject self, GameObject parent)
        {
            self.transform.SetParent(parent.transform);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this GameObject self, Transform parent)
        {
            self.transform.SetParent(parent);
        }

        /// <summary>
        /// Returns an ancestor from the transform's hierarchy on the specified hierarchy level.
        /// </summary>
        public static GameObject GetParent(this GameObject self, int level = 1)
        {
            return self.transform.GetParent(level)?.gameObject;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponentInParent<T>(this Component self, bool includeInactive)
        {
            return self.transform.GetComponentInParent<T>(includeInactive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponentInParent<T>(this GameObject self, bool includeInactive)
        {
            return self.transform.GetComponentInParent<T>(includeInactive);
        }

        public static T GetComponentInParent<T>(this Transform self, bool includeInactive)
        {
            if (!includeInactive)
                return self.GetComponentInParent<T>();

            for (var p = self; p != null; p = p.parent)
            {
                T component = p.GetComponent<T>();
                if (component != null)
                    return component;
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponentInChildren<T>(this Component self, bool includeInactive)
        {
            return self.transform.GetComponentInChildren<T>(includeInactive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponentInChildren<T>(this GameObject self, bool includeInactive)
        {
            return self.transform.GetComponentInChildren<T>(includeInactive);
        }

        public static T GetComponentInChildren<T>(this Transform self, bool includeInactive)
        {
            if (!includeInactive)
                return self.GetComponentInChildren<T>();

            foreach (var child in self.EnumerateChildren(true))
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform GetRectTransform(this GameObject self)
        {
            return self.transform as RectTransform;
        }

        /// <summary>
        /// Creates a sprite.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Returns triangles count of the mesh;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetTriangleCount(this Mesh self)
        {
            return self.triangles.Length / 3;
        }

        /// <summary>
        /// Returns vertex indices of the triangle of the mesh.
        /// </summary>
        public static (int i0, int i1, int i2) GetTriangleIndices(this Mesh self, int triangleNum)
        {
            int trIndex = triangleNum * 3;

            return
            (
                self.triangles[trIndex],
                self.triangles[++trIndex],
                self.triangles[++trIndex]
            );
        }

        /// <summary>
        /// Returns the triangle of the mesh.
        /// </summary>
        public static Plane GetTriangle(this Mesh self, int triangleNum)
        {
            (int i0, int i1, int i2) = self.GetTriangleIndices(triangleNum);
            Vector3[] vertices = self.vertices;

            return new Plane(vertices[i0], vertices[i1], vertices[i2]);
        }

        /// <summary>
        /// Returns true if a game object is prefab reference. For scene objects returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this GameObject self)
        {
            return !self.scene.IsValid();
        }

        /// <summary>
        /// Returns true if UnityEngine.Object is asset reference.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsset(this UnityObject self)
        {
            return self.GetInstanceID() > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetOrthographicSize(this Camera self)
        {
            return ScreenUtility.GetCameraViewRadius(self.orthographicSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPerspectiveSize(this Camera self, float distance)
        {
            return ScreenUtility.GetCameraViewRadius(self.fieldOfView, distance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetHorizontalFov(this Camera self)
        {
#if UNITY_2019_1_OR_NEWER
            return Camera.VerticalToHorizontalFieldOfView(self.fieldOfView, (float)Screen.width / Screen.height);
#else
            return ScreenUtility.GetAspectAngle(self.fieldOfView, (float)Screen.width / Screen.height);
#endif
        }
    }
}
