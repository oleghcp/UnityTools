using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OlegHcp.CSharp;

namespace OlegHcp.Tools
{
    public static class Helper
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            (b, a) = (a, b);
        }

        public static object GetDefaultValue(Type type)
        {
            TypeCode typeCode = type.GetTypeCode();

            switch (typeCode)
            {
                case TypeCode.Boolean: return default(bool);
                case TypeCode.Byte: return default(byte);
                case TypeCode.Char: return default(char);
                case TypeCode.DateTime: return default(DateTime);
                case TypeCode.Decimal: return default(decimal);
                case TypeCode.Double: return default(double);
                case TypeCode.Int16: return default(short);
                case TypeCode.Int32: return default(int);
                case TypeCode.Int64: return default(long);
                case TypeCode.SByte: return default(sbyte);
                case TypeCode.Single: return default(float);
                case TypeCode.UInt16: return default(ushort);
                case TypeCode.UInt32: return default(uint);
                case TypeCode.UInt64: return default(ulong);

                case TypeCode.Object:
                    if (type.IsValueType)
                        return Activator.CreateInstance(type);
                    return null;

                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.String:
                    return null;

                default: throw new SwitchExpressionException(typeCode);
            }
        }

#if UNITY
        public static object CloneObject(object source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            Type type = source.GetType();

            if (!type.IsSerializable)
                throw new SerializationException($"Type {type} is not marked as serializable.");

            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            };
        }
#endif

        internal static string SimplifyTypeName(string assemblyQualifiedName)
        {
            const char divider = ',';

            bool first = false;

            for (int i = 0; i < assemblyQualifiedName.Length; i++)
            {
                if (assemblyQualifiedName[i] != divider) { continue; }
                if (!first) { first = true; }
                else { return assemblyQualifiedName.Substring(0, i); }
            }

            return null;
        }
    }
}
