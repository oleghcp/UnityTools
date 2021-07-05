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
            if (!_dataTable.TryGetValue(self, out ObjectData data))
                _dataTable.Add(self, data = new ObjectData { Type = self.GetType() });

            if (!data.Methods.TryGetValue(methodName, out var method))
                data.Methods[methodName] = method = data.Type.GetMethod(methodName, MASK);

            return method?.Invoke(self, arg);
        }

        private class ObjectData
        {
            public Type Type;
            public Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
        }
    }
}
