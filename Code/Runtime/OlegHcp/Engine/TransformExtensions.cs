﻿using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections;
using UnityEngine;

namespace OlegHcp.Engine
{
    public static class TransformExtensions
    {
        public static Vector3 Back(this Transform self)
        {
            return self.rotation * Vector3.back;
        }

        public static Vector3 Left(this Transform self)
        {
            return self.rotation * Vector3.left;
        }

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
            self.SetLocalPositionAndRotation(targetLocalPos, targetLocalRot);
        }

        public static void SetParent(this Transform self, GameObject parent)
        {
            self.SetParent(parent.transform);
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
            self.SetLocalPositionAndRotation(localPos, localRot);
            self.localScale = localScl;
        }

        /// <summary>
        /// Copies local values from other transform.
        /// Warning: it will not work properly if transforms have different parents.
        /// </summary>
        public static void SetLocalParams(this Transform self, Transform source, bool includeScale = false)
        {
            self.SetLocalPositionAndRotation(source.localPosition, source.localRotation);
            if (includeScale)
                self.localScale = source.localScale;
        }

        public static void SetParams(this Transform self, Transform source)
        {
            self.SetPositionAndRotation(source.position, source.rotation);
        }

        public static void FromLocalToWorldParams(this Transform self, Transform source)
        {
            self.SetPositionAndRotation(source.localPosition, source.localRotation);
        }

        public static void FromWorldToParamsLocal(this Transform self, Transform source, bool includeScale = false)
        {
            self.SetLocalPositionAndRotation(source.position, source.rotation);
            if (includeScale)
                self.localScale = source.lossyScale;
        }

        public static void ResetLocalParams(this Transform self, bool includeScale = true)
        {
            self.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (includeScale)
                self.localScale = Vector3.one;
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
        /// Increases sibling index by one.
        /// </summary>
        public static void IncreaseSiblingIndex(this Transform self)
        {
            self.SetSiblingIndex(self.GetSiblingIndex() + 1);
        }

        /// <summary>
        /// Decreases sibling index by one.
        /// </summary>
        public static void DecreaseSiblingIndex(this Transform self)
        {
            self.SetSiblingIndex(self.GetSiblingIndex() - 1);
        }

        /// <summary>
        /// Increases or decreases sibling index by the specified value.
        /// </summary>
        public static void MoveSiblingIndex(this Transform self, int moveValue)
        {
            self.SetSiblingIndex(self.GetSiblingIndex() + moveValue);
        }

        /// <summary>
        /// Sorts children by selected key.
        /// </summary>
        public static void OrderChildren<TKey>(this Transform self, Func<Transform, TKey> keySelector)
        {
            Transform[] children = self.GetTopChildren();
            children.Sort(keySelector);
            TransformUtility.OrderSiblingsByList(children);
        }

        /// <summary>
        /// Sorts children by selected key.
        /// </summary>
        public static void OrderChildren(this Transform self, Comparison<Transform> comparer)
        {
            Transform[] children = self.GetTopChildren();
            children.Sort(comparer);
            TransformUtility.OrderSiblingsByList(children);
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
        public static void Move(this RectTransform self, float x, float y, Space relativeTo = default)
        {
            Move(self, new Vector2(x, y), relativeTo);
        }

        public static void SetSizeWithCurrentAnchors(this RectTransform self, in Vector2 size)
        {
            RectTransform rectTransform = self.parent as RectTransform;
            Vector2 parentSize = rectTransform == null ? Vector2.zero : rectTransform.rect.size;
            self.sizeDelta = size - parentSize * (self.anchorMax - self.anchorMin);
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
        public static RectTransform GetParent(this RectTransform self)
        {
            return self.parent as RectTransform;
        }

        public static void SetAnchor(this RectTransform self, TextAnchor anchor, bool setPivot)
        {
            Vector2 vector = RectUtility.GetAnchor(anchor);

            self.anchorMin = vector;
            self.anchorMax = vector;

            if (setPivot)
                self.pivot = vector;
        }

        public static void SetAnchor(this RectTransform self, RectTransformStretch stretch, bool setPivot)
        {
            Rect anchorRect = RectUtility.GetAnchor(stretch, out Vector2 pivot);

            self.anchorMin = anchorRect.min;
            self.anchorMax = anchorRect.max;

            if (setPivot)
                self.pivot = pivot;
        }

        public static void SetPivot(this RectTransform self, TextAnchor anchor)
        {
            self.pivot = RectUtility.GetAnchor(anchor);
        }

        /// <summary>
		/// Sets the left offset of a rect transform to the specified value
		/// </summary>
		public static void SetLeft(this RectTransform self, float left)
        {
            self.offsetMin = new Vector2(left, self.offsetMin.y);
        }

        /// <summary>
        /// Sets the right offset of a rect transform to the specified value
        /// </summary>
        public static void SetRight(this RectTransform self, float right)
        {
            self.offsetMax = new Vector2(-right, self.offsetMax.y);
        }

        /// <summary>
        /// Sets the top offset of a rect transform to the specified value
        /// </summary>
        public static void SetTop(this RectTransform self, float top)
        {
            self.offsetMax = new Vector2(self.offsetMax.x, -top);
        }

        /// <summary>
        /// Sets the bottom offset of a rect transform to the specified value
        /// </summary>
        public static void SetBottom(this RectTransform self, float bottom)
        {
            self.offsetMin = new Vector2(self.offsetMin.x, bottom);
        }
    }
}
