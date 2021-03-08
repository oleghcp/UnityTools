using System;
using System.Runtime.CompilerServices;
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
        public static bool IsAlive(object obj)
        {
            return !IsNullOrDead(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T CreateDelegate<T>(Type classType, string methodName) where T : Delegate
        {
            return Delegate.CreateDelegate(typeof(T), classType, methodName) as T;
        }
    }
}
