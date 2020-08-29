using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tools;
using UnityUtility;
using UnityObject = UnityEngine.Object;

namespace UnityEngine
{
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Description of scale transformation.
        /// </summary>
        public enum ScaleAction { ResetToOne, KeepAsLocal, KeepAsWorld }

        /// <summary>
        /// Calculates view bounds of the orthographic camera looking along the Z axis.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetViewBounds(this Camera self)
        {
            return ScreenUtility.GetViewBounds(self.transform.position, self.orthographicSize);
        }

        /// <summary>
        /// Calculates view bounds of the perspective camera looking along the Z axis.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetViewBounds(this Camera self, float distance)
        {
            return ScreenUtility.GetViewBounds(self.transform.position, self.fieldOfView, distance);
        }

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
        public static GameObject Install(this GameObject self, Transform parent, ScaleAction scaleAction = ScaleAction.KeepAsWorld)
        {
            GameObject go = UnityObject.Instantiate(self, parent, false);
            f_init(go.transform, scaleAction);
            return go;
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
        public static T Install<T>(this T self, Transform parent, ScaleAction scaleAction = ScaleAction.KeepAsWorld) where T : Component
        {
            T copy = UnityObject.Instantiate(self, parent, false);
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

            if (t is RectTransform rt)
                rt.anchoredPosition = Vector2.zero;
            else
                t.localPosition = Vector3.zero;
        }
        #endregion

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

        /// <summary>
        /// Makes detached from the parent.
        /// </summary>
        public static void Free(this Transform self, bool resetScale = false)
        {
            self.parent = null;
            if (resetScale) { self.localScale = Vector3.one; }
        }

        /// <summary>
        /// Makes detached from parent with the specified scale.
        /// </summary>
        public static void Free(this Transform self, Vector3 scale)
        {
            self.parent = null;
            self.localScale = scale;
        }

        /// <summary>
        /// Set the parent of the transform with locating to the specified local position.
        /// </summary>
        public static void SetParent(this Transform self, Transform parent, in Vector3 targetLocalPos)
        {
            self.SetParent(parent);
            self.localPosition = targetLocalPos;
        }

        /// <summary>
        /// Set the parent of the transform with the specified local position and rotation.
        /// </summary>
        public static void SetParent(this Transform self, Transform parent, in Vector3 targetLocalPos, in Quaternion targetLocalRot)
        {
            self.SetParent(parent);
            self.localPosition = targetLocalPos;
            self.localRotation = targetLocalRot;
        }

        public static void SetLocalParams(this Transform self, in Vector3 localPos, in Quaternion localRot)
        {
            self.localPosition = localPos;
            self.localRotation = localRot;
        }

        public static void SetLocalParams(this Transform self, in Vector3 localPos, in Quaternion localRot, in Vector3 localScl)
        {
            self.localPosition = localPos;
            self.localRotation = localRot;
            self.localScale = localScl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyParams(this Transform self, Transform source)
        {
            self.SetPositionAndRotation(source.position, source.rotation);
        }

        public static void CopyLocalParams(this Transform self, Transform source, bool withScale = false)
        {
            self.localPosition = source.localPosition;
            self.localRotation = source.localRotation;
            if (withScale)
                self.localScale = source.localScale;
        }

        /// <summary>
        /// Returns transform of gameobject as RectTransform if posible. Otherwise returns null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform GetRectTransform(this GameObject self)
        {
            return self.transform as RectTransform;
        }

        /// <summary>
        /// Set the parent of the rectTransform with locating to the specified anchored position.
        /// </summary>
        public static void SetParent(this RectTransform self, RectTransform parent, in Vector2 targetAnchPos)
        {
            self.SetParent(parent);
            self.anchoredPosition = targetAnchPos;
        }

        /// <summary>
        /// Returns an ancestor from the transform's hierarchy on the specified hierarchy level.
        /// </summary>
        public static Transform GetParent(this Transform self, int level)
        {
            Transform parent = self;

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
        public static Transform GetParent(this Transform self, Func<Transform, bool> condition)
        {
            Transform parent = self;

            while (parent != null && !condition(parent))
            {
                parent = parent.parent;
            }

            return parent;
        }

        /// <summary>
        /// Returns IEnumerable collection of parents;
        /// </summary>
        public static IEnumerable<Transform> EnumerateParents(this Transform self)
        {
            for (Transform p = self.parent; p != null; p = p.parent)
            {
                yield return p;
            }
        }

        /// <summary>
        /// Returns IEnumerable collection of all children;
        /// </summary>
        public static IEnumerable<Transform> EnumerateChildren(this Transform self, bool recursive = true)
        {
            int length = self.childCount;

            for (int i = 0; i < length; i++)
            {
                Transform child = self.GetChild(i);

                if (recursive)
                {
                    foreach (var subChild in child.EnumerateChildren())
                        yield return subChild;
                }

                yield return child;
            }
        }

        /// <summary>
        /// Returns children of the top level of the hierarchy.
        /// </summary>
        public static Transform[] GetTopChildren(this Transform self)
        {
            int count = self.childCount;

            Transform[] children = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                children[i] = self.GetChild(i);
            }

            return children;
        }

        /// <summary>
        /// Finds all children.
        /// </summary>
        public static IReadOnlyList<Transform> GetAllChildren(this Transform self)
        {
            List<Transform> list = new List<Transform>();

            GetAllChildren(self, list);

            return list;
        }

        /// <summary>
        /// Finds all children.
        /// </summary>
        public static void GetAllChildren(this Transform self, List<Transform> list)
        {
            int length = self.childCount;

            for (int i = 0; i < length; i++)
            {
                Transform ch = self.GetChild(i);

                list.Add(ch);

                GetAllChildren(ch, list);
            }
        }

        public static T GetComponentInParent<T>(this Transform self, bool includeInactive)
        {
            if (!includeInactive)
                return self.GetComponentInParent<T>();

            for (var p = self; p != null; p = p.parent)
            {
                var component = p.GetComponent<T>();
                if (component != null)
                    return component;
            }

            return default;
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
        public static Quaternion TransformRotation(this Transform self, in Quaternion rotation)
        {
            return self.rotation * rotation;
        }

        /// <summary>
        /// Moves the rect transform in the 2D direction and distance of translation.
        /// </summary>        
        /// <param name="translation">Translation vector.</param>
        /// <param name="relativeTo">Translation's relative space (world or self).</param>
        public static void Move(this RectTransform self, Vector2 translation, Space relativeTo = default)
        {
            if (relativeTo == Space.World) { self.anchoredPosition += translation; }
            else { self.anchoredPosition += self.TransformDirection(translation).XY(); }
        }

        /// <summary>
        /// Moves the rect transform in the 2D direction and distance of translation.
        /// </summary>
        /// <param name="relativeTo">Translation's relative space (world or self).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Move(this RectTransform self, float x, float y, Space relativeTo = default)
        {
            Move(self, new Vector2(x, y), relativeTo);
        }

        public static void SetSizeWithCurrentAnchors(this RectTransform self, in Vector2 size)
        {
            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        public static void SetSizeWithCurrentAnchors(this RectTransform self, in Rect rect)
        {
            Vector2 size = rect.size;
            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            self.pivot = rect.GetPivot();
        }

        /// <summary>
        /// Returns the parent of the rectTransform.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectTransform GetParent(this RectTransform self)
        {
            return self.parent as RectTransform;
        }

        public static void SetAnchor(this RectTransform self, TextAnchor anchor, bool setPivot = false)
        {
            Vector2 vector = RectUtility.GetAnchor(anchor);

            self.anchorMin = vector;
            self.anchorMax = vector;

            if (setPivot)
                self.pivot = vector;
        }

        public static void SetPivot(this RectTransform self, TextAnchor anchor)
        {
            Vector2 vector = RectUtility.GetAnchor(anchor);
            self.pivot = vector;
        }

        ///// <summary>
        ///// Sets the position of this RectTransform pivot relative to the anchor reference and its parent pivot.
        ///// </summary>
        //public static void SetLocalPos(this RectTransform self, in Vector2 localPos)
        //{
        //    Vector2 pivot = self.GetParent().pivot;
        //    Vector2 size = self.GetParent().rect.size;

        //    float xOffset = size.x * pivot.x - size.x * 0.5f;
        //    float yOffset = size.y * pivot.y - size.y * 0.5f;

        //    self.anchoredPosition = localPos + new Vector2(xOffset, yOffset);
        //}

        ///// <summary>
        ///// Sets the position of this RectTransform pivot relative to the anchor reference and custom parent pivot.
        ///// </summary>
        //public static void SetLocalPos(this RectTransform self, in Vector2 localPos, in Vector2 customParentPivot)
        //{
        //    Vector2 size = self.GetParent().rect.size;

        //    float xOffset = size.x * customParentPivot.x - size.x * 0.5f;
        //    float yOffset = size.y * customParentPivot.y - size.y * 0.5f;

        //    self.anchoredPosition = localPos + new Vector2(xOffset, yOffset);
        //}

        ///// <summary>
        ///// Returns the position of this RectTransform pivot relative to the anchor reference and its parent pivot.
        ///// </summary>
        //public static Vector2 GetLocalPos(this RectTransform self)
        //{
        //    Vector2 pivot = self.GetParent().pivot;
        //    Vector2 size = self.GetParent().rect.size;

        //    float xOffset = size.x * pivot.x - size.x * 0.5f;
        //    float yOffset = size.y * pivot.y - size.y * 0.5f;

        //    return self.anchoredPosition - new Vector2(xOffset, yOffset);
        //}

        ///// <summary>
        ///// Returns the position of this RectTransform pivot relative to the anchor reference and custom parent pivot.
        ///// </summary>
        //public static Vector2 GetLocalPos(this RectTransform self, in Vector2 customParentPivot)
        //{
        //    Vector2 size = self.GetParent().rect.size;

        //    float xOffset = size.x * customParentPivot.x - size.x * 0.5f;
        //    float yOffset = size.y * customParentPivot.y - size.y * 0.5f;

        //    return self.anchoredPosition - new Vector2(xOffset, yOffset);
        //}

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
        /// Returns true if prefab. For scene objects returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this GameObject self)
        {
            return !self.scene.IsValid();
        }

        /// <summary>
        /// Increases sibling index by one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncreaseSiblingIndex(this Transform self)
        {
            self.SetSiblingIndex(self.GetSiblingIndex() + 1);
        }

        /// <summary>
        /// Decreases sibling index by one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DecreaseSiblingIndex(this Transform self)
        {
            self.SetSiblingIndex(self.GetSiblingIndex() - 1);
        }

        /// <summary>
        /// Increases sibling index by the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncreaseSiblingIndex(this Transform self, int value)
        {
            if (value < 0)
                throw Errors.NegativeParameter(nameof(value));

            self.SetSiblingIndex(self.GetSiblingIndex() + value);
        }

        /// <summary>
        /// Decreases sibling index by the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DecreaseSiblingIndex(this Transform self, int value)
        {
            if (value < 0)
                throw Errors.NegativeParameter(nameof(value));

            self.SetSiblingIndex(self.GetSiblingIndex() - value);
        }
    }
}
