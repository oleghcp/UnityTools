using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityUtility;

namespace UnityEngine
{
    public static class TransformExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Back(this Transform self)
        {
            return self.rotation * Vector3.back;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Left(this Transform self)
        {
            return self.rotation * Vector3.left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Down(this Transform self)
        {
            return self.rotation * Vector3.down;
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

#if !UNITY_2022_1_OR_NEWER
        public static void SetLocalPositionAndRotation(this Transform self, in Vector3 localPos, in Quaternion localRot)
        {
            self.localPosition = localPos;
            self.localRotation = localRot;
        }
#endif

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent(this Transform self, GameObject parent)
        {
            self.SetParent(parent.transform);
        }

        /// <summary>
        /// Returns an ancestor from the transform's hierarchy on the specified hierarchy level.
        /// </summary>
        public static Transform GetParent(this Transform self, int level)
        {
            Transform target = self;

            for (int i = 0; i < level && target != null; i++)
            {
                target = target.parent;
            }

            return target;
        }

        /// <summary>
        /// Finds an ancestor in the transform's hierarchy satisfying the specified condition.
        /// </summary>
        public static Transform GetParent(this Transform self, Predicate<Transform> condition)
        {
            Transform parent = self.parent;

            while (parent != null)
            {
                if (condition(parent))
                    return parent;

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

        public static Transform GetRootTransform(this Transform self)
        {
            Transform root = self;

            for (Transform p = self.parent; p != null; p = p.parent)
            {
                root = p;
            }

            return root;
        }

        /// <summary>
        /// Returns IEnumerable collection of all children;
        /// </summary>
        public static IEnumerable<Transform> EnumerateChildren(this Transform self, bool recursive)
        {
            int count = self.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform child = self.GetChild(i);

                yield return child;

                if (recursive)
                {
                    foreach (Transform subChild in child.EnumerateChildren(true))
                    {
                        yield return subChild;
                    }
                }
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
        public static List<Transform> GetAllChildren(this Transform self)
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
            int count = self.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform ch = self.GetChild(i);

                list.Add(ch);

                GetAllChildren(ch, list);
            }
        }

        public static void OrderSiblings<TKey>(this Transform self, Func<Transform, TKey> keySelector)
        {
            Transform[] children = self.GetTopChildren();
            children.Sort(keySelector);
            TransformUtility.OrderSiblingsByList(children);
        }

        public static void DestroyChildren(this Transform self)
        {
            Transform[] children = self.GetTopChildren();

            for (int i = 0; i < children.Length; i++)
            {
                children[i].gameObject.Destroy();
            }
        }

        public static void DestroyChildren(this Transform self, Predicate<Transform> predicate)
        {
            Transform[] children = GetSelectedChildren(self, predicate);

            for (int i = 0; i < children.Length; i++)
            {
                children[i].gameObject.Destroy();
            }
        }

        public static void DetachChildren(this Transform self, Predicate<Transform> predicate)
        {
            Transform[] children = GetSelectedChildren(self, predicate);

            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetParent(null);
            }
        }

        private static Transform[] GetSelectedChildren(Transform self, Predicate<Transform> predicate)
        {
            return self.EnumerateChildren(false)
                       .Where(item => predicate(item))
                       .ToArray();
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
        /// Increases or decreases sibling index by the specified value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveSiblingIndex(this Transform self, int moveValue)
        {
            self.SetSiblingIndex(self.GetSiblingIndex() + moveValue);
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

        /// <summary>
        /// Set the parent of the rectTransform with locating to the specified anchored position.
        /// </summary>
        public static void SetParent(this RectTransform self, RectTransform parent, in Vector2 targetAnchPos)
        {
            self.SetParent(parent);
            self.anchoredPosition = targetAnchPos;
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
    }
}
