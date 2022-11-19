using System;

namespace UnityEngine
{
    public static class GetComponentExtensions
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
        public static bool TryGetComponent<T>(this GameObject self, out T component) where T : Component
        {
            component = self.GetComponent<T>();
            return component != null;
        }

        public static bool TryGetComponent(this GameObject self, Type type, out Component component)
        {
            component = self.GetComponent(type);
            return component != null;
        }

        public static bool TryGetComponent<T>(this Component self, out T component) where T : Component
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
