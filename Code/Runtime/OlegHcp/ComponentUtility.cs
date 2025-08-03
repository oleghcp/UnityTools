using System.Runtime.CompilerServices;
using OlegHcp.Engine;
using UnityEngine;

namespace OlegHcp
{
    public static class ComponentUtility
    {
        /// <summary>
        /// Creates and returns an instance of GameObject with Component of T type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        public static T CreateInstance<T>(string gameObjectName) where T : Component
        {
            GameObject go = new GameObject(gameObjectName);
            return go.AddComponent(typeof(T)) as T;
        }

        /// <summary>
        /// Creates and returns an instance of GameObject with Component of T type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateInstance<T>() where T : Component
        {
            return CreateInstance<T>(typeof(T).Name);
        }

        /// <summary>
        /// Creates and returns an instance of GameObject with Component of T type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        public static T CreateInstance<T>(string gameObjectName, Transform parent) where T : Component
        {
            T component = CreateInstance<T>(gameObjectName);
            component.transform.SetParent(parent);
            return component;
        }

        /// <summary>
        /// Creates and returns an instance of GameObject with Component of T type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        public static T CreateInstance<T>(Transform parent) where T : Component
        {
            return CreateInstance<T>(typeof(T).Name, parent);
        }

        public static T CreateInstance<T>(in Vector3 position) where T : Component
        {
            return CreateInstance<T>(typeof(T).Name, position);
        }

        public static T CreateInstance<T>(in Vector3 position, in Quaternion rotation) where T : Component
        {
            return CreateInstance<T>(typeof(T).Name, position, rotation);
        }

        public static T CreateInstance<T>(Transform parent, in Vector3 position, in Quaternion rotation, bool local) where T : Component
        {
            return CreateInstance<T>(typeof(T).Name, parent, position, rotation, local);
        }

        public static T CreateInstance<T>(string gameObjectName, in Vector3 position) where T : Component
        {
            T component = CreateInstance<T>(gameObjectName);
            component.transform.position = position;
            return component;
        }

        public static T CreateInstance<T>(string gameObjectName, in Vector3 position, in Quaternion rotation) where T : Component
        {
            T component = CreateInstance<T>(gameObjectName);
            component.transform.SetPositionAndRotation(position, rotation);
            return component;
        }

        public static T CreateInstance<T>(string gameObjectName, Transform parent, in Vector3 position, in Quaternion rotation, bool local) where T : Component
        {
            T component = CreateInstance<T>(gameObjectName);

            if (local)
            {
                component.transform.SetParent(parent);
                component.transform.SetLocalPositionAndRotation(position, rotation);
            }
            else
            {
                component.transform.SetPositionAndRotation(position, rotation);
                component.transform.SetParent(parent);
            }

            return component;
        }
    }
}
