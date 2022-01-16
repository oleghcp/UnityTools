using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    public static class Messaging
    {
        private const BindingFlags MASK = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static ConditionalWeakTable<object, ObjectData> _dataTable = new ConditionalWeakTable<object, ObjectData>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object SendMsg(this object self, string methodName)
        {
            return SendMsg(self, methodName, null);
        }

        public static object SendMsg(this object self, string methodName, params object[] arg)
        {
            ObjectData data = _dataTable.GetValue(self, CreateValue);

            if (!data.Methods.TryGetValue(methodName, out MethodInfo method))
                data.Methods[methodName] = method = data.Type.GetMethod(methodName, MASK);

            return method?.Invoke(self, arg);
        }

        private static ObjectData CreateValue(object key)
        {
            return new ObjectData { Type = key.GetType() };
        }

        private class ObjectData
        {
            public Type Type;
            public Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
        }
    }
}
