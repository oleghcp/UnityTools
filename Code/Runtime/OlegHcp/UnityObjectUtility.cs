using System;
using System.Runtime.CompilerServices;
using OlegHcp.CSharp;
using OlegHcp.Tools;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcp
{
    public static class UnityObjectUtility
    {
        private static readonly Func<UnityObject, bool> _isNativeObjectAlive = Helper.CreateDelegate<Func<UnityObject, bool>>(typeof(UnityObject), "IsNativeObjectAlive");

        public static bool IsNullOrDead(object obj)
        {
            if (obj == null)
                return true;

            if (obj is UnityObject uObj)
                return !_isNativeObjectAlive(uObj);

            return false;
        }

        public static bool ExistsAndAlive(object obj)
        {
            if (obj == null)
                return false;

            if (obj is UnityObject uObj)
                return _isNativeObjectAlive(uObj);

            return true;
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
#if UNITY_2020_3_OR_NEWER
            return !UnityObject.FindObjectsByType(obj.GetType(), FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                               .Contains(obj);
#else
            return !UnityObject.FindObjectsOfType(obj.GetType()).Contains(obj);
#endif
        }
    }
}
