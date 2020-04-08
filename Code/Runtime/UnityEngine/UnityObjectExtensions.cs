using UnityObject = UnityEngine.Object;
using System;
using System.Collections.Generic;
using UnityUtility;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Description of scale transformation.
        /// </summary>
        public enum ScaleAction { ResetToOne, KeepAsLocal, KeepAsGlobal }

        /// <summary>
        /// Calculates view bounds of the orthographic camera looking along the Z axis.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetViewBounds(this Camera cam)
        {
            return ScreenUtility.GetViewBounds(cam.transform.position, cam.orthographicSize);
        }

        /// <summary>
        /// Calculates view bounds of the perspective camera looking along the Z axis.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetViewBounds(this Camera cam, float distance)
        {
            return ScreenUtility.GetViewBounds(cam.transform.position, cam.fieldOfView, distance);
        }

        /// <summary>
        /// Destroys the unity object.
        /// </summary>        
        /// <param name="time">Time  for destruction.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(this UnityObject obj, float time = 0f)
        {
            UnityObject.Destroy(obj, time);
        }

        /// <summary>
        /// Instantiates unity object of defined type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Install<T>(this T obj) where T : UnityObject
        {
            return UnityObject.Instantiate(obj);
        }

        /// <summary>
        /// Instantiates gameobject as a child of the specified parent.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Install(this GameObject obj, Transform parent, bool worldPositionStays)
        {
            return UnityObject.Instantiate(obj, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates gameobject as a child with default local position and rotation.
        /// </summary>
        public static GameObject Install(this GameObject obj, Transform parent, ScaleAction scaleAction = ScaleAction.KeepAsGlobal)
        {
            GameObject go = UnityObject.Instantiate(obj, parent, false);
            f_init(go.transform, scaleAction);
            return go;
        }

        /// <summary>
        /// Instantiates gameobject as a child of the specified parent.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Install<T>(this T comp, Transform parent, bool worldPositionStays) where T : Component
        {
            return UnityObject.Instantiate(comp, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates defined component as a child with default local position and rotation.
        /// </summary>
        public static T Install<T>(this T comp, Transform parent, ScaleAction scaleAction = ScaleAction.KeepAsGlobal) where T : Component
        {
            T copy = UnityObject.Instantiate(comp, parent, false);
            f_init(copy.transform, scaleAction);
            return copy;
        }

        #region Install help func
        private static void f_init(Transform t, ScaleAction scaleAction)
        {
            t.localRotation = Quaternion.identity;

            switch (scaleAction)
            {
                case ScaleAction.ResetToOne:
                    t.localScale = Vector3.one;
                    break;

                case ScaleAction.KeepAsLocal:
                    t.localScale = t.lossyScale;
                    break;
            }

            if (t is RectTransform)
            {
                (t as RectTransform).anchoredPosition = Vector2.zero;
            }
            else
            {
                t.localPosition = Vector3.zero;
            }
        }
        #endregion

        /// <summary>
        /// Instantiates gameobject to the specified position with the specified rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Install(this GameObject obj, in Vector3 position, in Quaternion rotation)
        {
            return UnityObject.Instantiate(obj, position, rotation);
        }

        /// <summary>
        /// Instantiates gameobject as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static GameObject Install(this GameObject obj, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local = true)
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(obj, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Instantiates defined component to specified position with specified rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Install<T>(this T comp, in Vector3 position, in Quaternion rotation) where T : Component
        {
            return UnityObject.Instantiate(comp, position, rotation);
        }

        /// <summary>
        /// Instantiates gameobject as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static T Install<T>(this T comp, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local = true) where T : Component
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(comp, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Returns existing component or adds and returns new one.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            var component = obj.GetComponent<T>();
            if (component == null)
                return obj.AddComponent<T>();
            return component;
        }

        /// <summary>
        /// Returns existing component of Type type or adds and returns new one.
        /// </summary>
        public static Component GetOrAddComponent(this GameObject obj, Type type)
        {
            var component = obj.GetComponent(type);
            if (component == null)
                return obj.AddComponent(type);
            return component;
        }

        /// <summary>
        /// Marks unity object as DontDestroyOnLoad.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Immortalize(this UnityObject obj)
        {
            UnityObject.DontDestroyOnLoad(obj);
        }

        /// <summary>
        /// Makes detached from the parent.
        /// </summary>
        public static void Free(this Transform t, bool resetScale = false)
        {
            t.parent = null;
            if (resetScale) { t.localScale = Vector3.one; }
        }

        /// <summary>
        /// Makes detached from parent with the specified scale.
        /// </summary>
        public static void Free(this Transform t, Vector3 scale)
        {
            t.parent = null;
            t.localScale = scale;
        }

        /// <summary>
        /// Set the parent of the transform with locating to the specified local position.
        /// </summary>
        public static void SetParent(this Transform t, Transform parent, in Vector3 targetLocalPos)
        {
            t.SetParent(parent);
            t.localPosition = targetLocalPos;
        }

        /// <summary>
        /// Set the parent of the transform with the specified local position and rotation.
        /// </summary>
        public static void SetParent(this Transform t, Transform parent, in Vector3 targetLocalPos, in Quaternion targetLocalRot)
        {
            t.SetParent(parent);
            t.localPosition = targetLocalPos;
            t.localRotation = targetLocalRot;
        }

        public static void SetLocalParams(this Transform t, in Vector3 localPos, in Quaternion localRot)
        {
            t.localPosition = localPos;
            t.localRotation = localRot;
        }

        public static void SetLocalParams(this Transform t, in Vector3 localPos, in Quaternion localRot, in Vector3 localScl)
        {
            t.localPosition = localPos;
            t.localRotation = localRot;
            t.localScale = localScl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyParams(this Transform target, Transform source)
        {
            target.SetPositionAndRotation(source.position, source.rotation);
        }

        public static void CopyLocalParams(this Transform target, Transform source, bool withScale = false)
        {
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            if (withScale)
                target.localScale = source.localScale;
        }

        /// <summary>
        /// Returns transform of gameobject as RectTransform if posible. Otherwise returns null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform GetRectTransform(this GameObject go)
        {
            return go.transform as RectTransform;
        }

        /// <summary>
        /// Set the parent of the rectTransform with locating to the specified anchored position.
        /// </summary>
        public static void SetParent(this RectTransform t, RectTransform parent, in Vector2 targetAnchPos)
        {
            t.SetParent(parent);
            t.anchoredPosition = targetAnchPos;
        }

        /// <summary>
        /// Returns an ancestor from the transform's hierarchy on the specified hierarchy level.
        /// </summary>
        public static Transform GetParent(this Transform t, int level)
        {
            Transform parent = t;

            for (int i = 0; i < level; i++)
            {
                if (parent.parent == null) { break; }
                parent = parent.parent;
            }

            return parent;
        }

        /// <summary>
        /// Finds an ancestor in the transform's hierarchy satisfying the specified condition.
        /// </summary>
        public static Transform GetParent(this Transform t, Func<Transform, bool> condition)
        {
            Transform parent = t;

            while (parent != null && !condition(parent))
            {
                parent = parent.parent;
            }

            return parent;
        }

        /// <summary>
        /// Returns children of the top level of the hierarchy.
        /// </summary>
        public static Transform[] GetTopChildren(this Transform t)
        {
            int count = t.childCount;

            Transform[] children = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                children[i] = t.GetChild(i);
            }

            return children;
        }

        /// <summary>
        /// Finds all children.
        /// </summary>
        public static Transform[] GetAllChildren(this Transform t)
        {
            List<Transform> list = new List<Transform>();

            GetAllChildren(t, list);

            return list.ToArray();
        }

        /// <summary>
        /// Finds all children.
        /// </summary>
        public static void GetAllChildren(this Transform t, List<Transform> list)
        {
            int length = t.childCount;

            for (int i = 0; i < length; i++)
            {
                Transform ch = t.GetChild(i);

                list.Add(ch);

                GetAllChildren(ch, list);
            }
        }

        public static T GetComponentInParent<T>(this Transform self, bool includeInactive) where T : Component
        {
            if (!includeInactive)
                return self.GetComponentInParent<T>();

            for (var p = self; p != null; p = p.parent)
            {
                var component = p.GetComponent<T>();
                if (component != null)
                    return component;
            }

            return null;
        }

        public static void DestroyChildren(this Transform self)
        {
            int length = self.childCount;

            for (int i = 0; i < length; i++)
            {
                self.GetChild(i).gameObject.Destroy();
            }
        }

        /// <summary>
        /// Transforms rotation from local space to world space.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion TransformRotation(this Transform t, in Quaternion rotation)
        {
            return t.rotation * rotation;
        }

        /// <summary>
        /// Moves the rect transform in the 2D direction and distance of translation.
        /// </summary>        
        /// <param name="translation">Translation vector.</param>
        /// <param name="relativeTo">Translation's relative space (world or self).</param>
        public static void Move(this RectTransform rt, Vector2 translation, Space relativeTo = default)
        {
            if (relativeTo == Space.World) { rt.anchoredPosition += translation; }
            else { rt.anchoredPosition += rt.TransformDirection(translation).XY(); }
        }

        /// <summary>
        /// Moves the rect transform in the 2D direction and distance of translation.
        /// </summary>
        /// <param name="relativeTo">Translation's relative space (world or self).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Move(this RectTransform rt, float x, float y, Space relativeTo = default)
        {
            Move(rt, new Vector2(x, y), relativeTo);
        }

        public static void SetSizeWithCurrentAnchors(this RectTransform rt, in Vector2 size)
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        public static void SetSizeWithCurrentAnchors(this RectTransform rt, in Rect rect)
        {
            Vector2 size = rect.size;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            rt.pivot = rect.GetPivot();
        }

        /// <summary>
        /// Returns the parent of the rectTransform.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform GetParent(this RectTransform t)
        {
            return t.parent as RectTransform;
        }

        /// <summary>
        /// Sets the position of this RectTransform pivot relative to the anchor reference and its parent pivot.
        /// </summary>
        public static void SetLocalPos(this RectTransform t, in Vector2 localPos)
        {
            Vector2 pivot = t.GetParent().pivot;
            Vector2 size = t.GetParent().rect.size;

            float xOffset = size.x * pivot.x - size.x * 0.5f;
            float yOffset = size.y * pivot.y - size.y * 0.5f;

            t.anchoredPosition = localPos + new Vector2(xOffset, yOffset);
        }

        /// <summary>
        /// Sets the position of this RectTransform pivot relative to the anchor reference and custom parent pivot.
        /// </summary>
        public static void SetLocalPos(this RectTransform t, in Vector2 localPos, in Vector2 customParentPivot)
        {
            Vector2 size = t.GetParent().rect.size;

            float xOffset = size.x * customParentPivot.x - size.x * 0.5f;
            float yOffset = size.y * customParentPivot.y - size.y * 0.5f;

            t.anchoredPosition = localPos + new Vector2(xOffset, yOffset);
        }

        /// <summary>
        /// Returns the position of this RectTransform pivot relative to the anchor reference and its parent pivot.
        /// </summary>
        public static Vector2 GetLocalPos(this RectTransform t)
        {
            Vector2 pivot = t.GetParent().pivot;
            Vector2 size = t.GetParent().rect.size;

            float xOffset = size.x * pivot.x - size.x * 0.5f;
            float yOffset = size.y * pivot.y - size.y * 0.5f;

            return t.anchoredPosition - new Vector2(xOffset, yOffset);
        }

        /// <summary>
        /// Returns the position of this RectTransform pivot relative to the anchor reference and custom parent pivot.
        /// </summary>
        public static Vector2 GetLocalPos(this RectTransform t, in Vector2 customParentPivot)
        {
            Vector2 size = t.GetParent().rect.size;

            float xOffset = size.x * customParentPivot.x - size.x * 0.5f;
            float yOffset = size.y * customParentPivot.y - size.y * 0.5f;

            return t.anchoredPosition - new Vector2(xOffset, yOffset);
        }

        /// <summary>
        /// Creates a sprite.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sprite ToSprite(this Texture2D tex, in Vector2 pivot, float pixelsPerUnit = 100f, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.Tight, in Vector4 border = default)
        {
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), pivot, pixelsPerUnit, extrude, meshType, border);
        }

        /// <summary>
        /// Creates a sprite with the pivot in the center.
        /// </summary>
        public static Sprite ToSprite(this Texture2D tex, float pixelsPerUnit = 100f, uint extrude = 0, SpriteMeshType meshType = SpriteMeshType.Tight, in Vector4 border = default)
        {
            float x = tex.width;
            float y = tex.height;
            return Sprite.Create(tex, new Rect(0f, 0f, x, y), new Vector2(x * 0.5f, y * 0.5f), pixelsPerUnit, extrude, meshType, border);
        }

        /// <summary>
        /// Returns triangles count of the mesh;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetTriangleCount(this Mesh mesh)
        {
            return mesh.triangles.Length / 3;
        }

        /// <summary>
        /// Returns vertex indices of the triangle of the mesh.
        /// </summary>
        public static Vector3Int GetTriangleIndices(this Mesh mesh, int triangleNum)
        {
            int trIndex = triangleNum * 3;

            return new Vector3Int
            {
                x = mesh.triangles[trIndex],
                y = mesh.triangles[++trIndex],
                z = mesh.triangles[++trIndex]
            };
        }

        /// <summary>
        /// Returns the triangle of the mesh.
        /// </summary>
        public static Plane GetTriangle(this Mesh mesh, int triangleNum)
        {
            Vector3Int indices = mesh.GetTriangleIndices(triangleNum);
            Vector3[] vertices = mesh.vertices;

            return new Plane(vertices[indices.x], vertices[indices.y], vertices[indices.z]);
        }

        /// <summary>
        /// Returns true if prefab. For scene objects returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this GameObject obj)
        {
            return !obj.scene.IsValid();
        }
    }
}
