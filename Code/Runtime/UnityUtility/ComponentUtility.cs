using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility
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
    }
}
