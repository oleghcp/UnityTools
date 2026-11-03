using System;
using UnityEngine;

namespace OlegHcp.Engine
{
    public static class GetOrAddComponentExtensions
    {
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
    }
}
