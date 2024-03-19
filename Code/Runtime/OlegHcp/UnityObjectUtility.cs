using System;
using System.Runtime.CompilerServices;
using OlegHcp.CSharp;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcp
{
    public static class UnityObjectUtility
    {
        private static readonly Func<UnityObject, bool> _isNativeObjectAlive = CreateDelegate<Func<UnityObject, bool>>(typeof(UnityObject), "IsNativeObjectAlive");

        public static bool IsNullOrDead(object obj)
        {
            if (obj == null)
                return true;

            if (obj is UnityObject uObj)
                return !_isNativeObjectAlive.Invoke(uObj);

            return false;
        }

        private static T CreateDelegate<T>(Type classType, string methodName) where T : Delegate
        {
            return (T)Delegate.CreateDelegate(typeof(T), classType, methodName);
        }

        /// <summary>
        /// Returns true if a game object is asset reference. For scene objects returns false.
        /// </summary>
        public static bool IsAsset(GameObject gameObject)
        {
            return !gameObject.scene.IsValid();
        }

        /// <summary>
        /// Returns true if the component is asset reference. For scene objects returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsset(Component component)
        {
            return IsAsset(component.gameObject);
        }

        /// <summary>
        /// Returns true if ScriptableObject is asset reference.
        /// </summary>
        public static bool IsAsset(ScriptableObject obj)
        {
            return !UnityObject.FindObjectsByType(obj.GetType(), FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                               .Contains(obj);
        }
    }
}
