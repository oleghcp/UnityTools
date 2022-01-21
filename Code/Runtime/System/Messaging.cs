#if !UNITY_EDITOR
using System.Collections.Generic;
#endif
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    public static class Messaging
    {
        private const BindingFlags MASK = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

#if !UNITY_EDITOR
        private static ConditionalWeakTable<object, ObjectData> _dataTable = new ConditionalWeakTable<object, ObjectData>();
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object SendMsg<T>(this T self, string methodName)
        {
            return SendMsg(self, methodName, null);
        }

        public static object SendMsg<T>(this T self, string methodName, params object[] arg)
        {
#if UNITY_EDITOR
            return typeof(T).GetMethod(methodName, MASK)?.Invoke(self, arg);
#else
            return _dataTable.GetValue(self, key => new ObjectData(typeof(T)))
                             .GetMethod(methodName)?
                             .Invoke(self, arg);
#endif
        }

#if !UNITY_EDITOR
        private class ObjectData
        {
            private readonly Dictionary<string, MethodInfo> _methods;
            private readonly Type _type;

            public ObjectData(Type type)
            {
                _type = type;
                _methods = new Dictionary<string, MethodInfo>();
            }

            public MethodInfo GetMethod(string methodName)
            {
                if (_methods.TryGetValue(methodName, out MethodInfo value))
                    return value;

                return _methods.Place(methodName, _type.GetMethod(methodName, MASK));
            }
        }
#endif
    }
}
