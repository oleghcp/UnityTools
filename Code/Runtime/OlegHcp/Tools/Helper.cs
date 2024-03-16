using System;
using System.Runtime.CompilerServices;
using OlegHcp.CSharp;

namespace OlegHcp.Tools
{
    public static class Helper
    {
        public static readonly string Space = " ";

        public static void Swap<T>(ref T a, ref T b)
        {
            (b, a) = (a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(int hc0, int hc1)
        {
#if UNITY_2021_2_OR_NEWER
            return HashCode.Combine(hc0, hc1);
#else
            return hc0 ^ hc1 << 2;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(int hc0, int hc1, int hc2)
        {
#if UNITY_2021_2_OR_NEWER
            return HashCode.Combine(hc0, hc1, hc2);
#else
            return hc0 ^ hc1 << 2 ^ hc2 >> 2;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(int hc0, int hc1, int hc2, int hc3)
        {
#if UNITY_2021_2_OR_NEWER
            return HashCode.Combine(hc0, hc1, hc2, hc3);
#else
            return hc0 ^ hc1 << 2 ^ hc2 >> 2 ^ hc3 >> 1;
#endif
        }

        internal static string SimplifyTypeName(string assemblyQualifiedName)
        {
            const char devider = ',';

            bool first = false;

            for (int i = 0; i < assemblyQualifiedName.Length; i++)
            {
                if (assemblyQualifiedName[i] != devider) { continue; }
                if (!first) { first = true; }
                else { return assemblyQualifiedName.Substring(0, i); }
            }

            return null;
        }

        public static object GetDefaultValue(Type type)
        {
            switch (type.GetTypeCode())
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

                default: throw new UnsupportedValueException(type.GetTypeCode());
            }
        }
    }
}
