using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public static class GameObjectExtensions
    {
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
        /// Returns transform of gameobject as RectTransform if posible. Otherwise returns null.
        /// </summary>
        public static RectTransform GetRectTransform(this GameObject self)
        {
            return self.transform as RectTransform;
        }
    }
}
