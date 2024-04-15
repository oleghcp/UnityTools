using System;
using System.Collections.Generic;
using UnityEngine;

namespace OlegHcp.Engine
{
    public static class GameObjectExtensions
    {
        public static void SetParent(this GameObject self, GameObject parent)
        {
            self.transform.SetParent(parent.transform);
        }

        public static void SetParent(this GameObject self, Transform parent)
        {
            self.transform.SetParent(parent);
        }

        /// <summary>
        /// Returns an ancestor from the transform's hierarchy.
        /// </summary>
        public static GameObject GetParent(this GameObject self)
        {
            Transform parent = self.transform.parent;
            return parent == null ? null : parent.gameObject;
        }

        /// <summary>
        /// Returns an ancestor from the transform's hierarchy on the specified hierarchy level.
        /// </summary>
        public static GameObject GetParent(this GameObject self, int level)
        {
            Transform parent = self.transform.GetParent(level);
            return parent == null ? null : parent.gameObject;
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

        public static void DestroyChildren(this GameObject self)
        {
            self.transform.DestroyChildren();
        }

        public static void DestroyChildren(this GameObject self, Predicate<Transform> predicate)
        {
            self.transform.DestroyChildren(predicate);
        }

        public static void DetachChildren(this GameObject self)
        {
            self.transform.DetachChildren();
        }

        public static void DetachChildren(this GameObject self, Predicate<Transform> predicate)
        {
            self.transform.DetachChildren(predicate);
        }

        /// <summary>
        /// Returns transform of game object as RectTransform if possible. Otherwise returns null.
        /// </summary>
        public static RectTransform GetRectTransform(this GameObject self)
        {
            return self.transform as RectTransform;
        }
    }
}
