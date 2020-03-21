using System;
using System.Runtime.CompilerServices;
using UnityObject = UnityEngine.Object;

namespace UU
{
    public static class UnityObjectUtility
    {
        private static readonly Func<UnityObject, bool> s_isNativeObjectAlive;

        static UnityObjectUtility()
        {
            s_isNativeObjectAlive = f_createDelegate<Func<UnityObject, bool>>(typeof(UnityObject), nameof(s_isNativeObjectAlive));
        }

        public static bool IsNullOrDead(object obj)
        {
            if (obj == null)
                return true;

            if (obj is UnityObject uObj)
                return s_isNativeObjectAlive(uObj);

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlive(object obj)
        {
            return !IsNullOrDead(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T f_createDelegate<T>(Type classType, string methodName) where T : Delegate
        {
            return Delegate.CreateDelegate(typeof(T), classType, methodName) as T;
        }
    }
}
