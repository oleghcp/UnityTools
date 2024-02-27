using System.Runtime.CompilerServices;
using UnityEngine;

namespace OlegHcp
{
    public static class ComponentUtility
    {
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
        public static T CreateInstance<T>(string gameObjectName) where T : Component
        {
            GameObject go = new GameObject(gameObjectName);
            return go.AddComponent(typeof(T)) as T;
        }

        /// <summary>
        /// Creates and returns an instance of GameObject with Component of T type.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        public static T CreateInstance<T>(Transform parent) where T : Component
        {
            T component = CreateInstance<T>();
            component.transform.SetParent(parent);
            return component;
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
    }
}
