#if !UNITY_EDITOR
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endif
using System.Reflection;

namespace System
{
    public static class Messaging
    {
        private const BindingFlags MASK = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

#if !UNITY_EDITOR
        private static ConditionalWeakTable<object, ObjectData> _dataTable = new ConditionalWeakTable<object, ObjectData>();
#endif

        public static object SendMsg(this object self, string methodName)
        {
            return SendMsg(self, methodName, null);
        }

        public static object SendMsg(this object self, string methodName, params object[] arg)
        {
#if UNITY_EDITOR
            return self.GetType().GetMethod(methodName, MASK)?.Invoke(self, arg);
#else
            return _dataTable.GetValue(self, key => new ObjectData(key.GetType()))
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
