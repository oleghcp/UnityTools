using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtility
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExistsAndAlive(object obj)
        {
            return !IsNullOrDead(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T CreateDelegate<T>(Type classType, string methodName) where T : Delegate
        {
            return (T)Delegate.CreateDelegate(typeof(T), classType, methodName);
        }

        /// <summary>
        /// Returns true if a game object is asset reference. For scene objects returns false.
        /// </summary>
        public static bool IsAsset(GameObject self)
        {
            return !self.scene.IsValid();
        }

        /// <summary>
        /// Returns true if a Component is asset reference. For scene objects returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsset(Component self)
        {
            return IsAsset(self.gameObject);
        }

        /// <summary>
        /// Returns true if UnityEngine.Object is asset reference.
        /// </summary>
        public static bool IsAsset(UnityObject self)
        {
            return self.GetInstanceID() > 0;
        }
    }
}
