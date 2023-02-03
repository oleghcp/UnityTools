using System;
using UnityEngine;

namespace UnityUtility.Engine
{
    public static class GetOrAddComponentExtensions
    {
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
                if (p.TryGetComponent(out T component))
                    return component;
            }

            return default;
        }

        public static Component GetComponentInParent(this Component self, Type type, bool includeInactive)
        {
            return self.transform.GetComponentInParent(type, includeInactive);
        }

        public static Component GetComponentInParent(this GameObject self, Type type, bool includeInactive)
        {
            return self.transform.GetComponentInParent(type, includeInactive);
        }

        public static Component GetComponentInParent(this Transform self, Type type, bool includeInactive)
        {
            if (!includeInactive)
                return self.GetComponentInParent(type);

            for (Transform p = self; p != null; p = p.parent)
            {
                if (p.TryGetComponent(type, out Component component))
                    return component;
            }

            return default;
        }
#endif

        public static Component AddComponent(this Component self, Type type)
        {
            return self.gameObject.AddComponent(type);
        }

        public static T AddComponent<T>(this Component self) where T : Component
        {
            return self.gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Returns existing component or adds and returns new one.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            if (self.TryGetComponent(out T component))
                return component;

            return self.AddComponent<T>();
        }

        /// <summary>
        /// Returns existing component or adds and returns new one.
        /// </summary>
        public static T GetOrAddComponent<T>(this Component self) where T : Component
        {
            return self.gameObject.GetOrAddComponent<T>();
        }

        /// <summary>
        /// Returns existing component of Type type or adds and returns new one.
        /// </summary>
        public static Component GetOrAddComponent(this GameObject self, Type type)
        {
            if (self.TryGetComponent(type, out Component component))
                return component;

            return self.AddComponent(type);
        }

        /// <summary>
        /// Returns existing component of Type type or adds and returns new one.
        /// </summary>
        public static Component GetOrAddComponent(this Component self, Type type)
        {
            return self.gameObject.GetOrAddComponent(type);
        }

#if !UNITY_2019_2_OR_NEWER
        public static bool TryGetComponent<T>(this GameObject self, out T component)
        {
            component = self.GetComponent<T>();
            return component != null;
        }

        public static bool TryGetComponent(this GameObject self, Type type, out Component component)
        {
            component = self.GetComponent(type);
            return component != null;
        }

        public static bool TryGetComponent<T>(this Component self, out T component)
        {
            component = self.GetComponent<T>();
            return component != null;
        }

        public static bool TryGetComponent(this Component self, Type type, out Component component)
        {
            component = self.GetComponent(type);
            return component != null;
        }
#endif
    }
}
