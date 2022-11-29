#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityUtility.CSharp.Collections;
using UnityUtility.CSharp.Runtime.CompilerServices;
#endif
using System.Reflection;

namespace UnityUtility
{
    public static class Messaging
    {
        private const BindingFlags MASK = BindingFlags.Public | BindingFlags.Instance;

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
            if (!_dataTable.TryGetValue(self, out ObjectData data))
                data = _dataTable.Place(self, new ObjectData(self.GetType()));

            return data.GetMethod(methodName)?.Invoke(self, arg);
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
