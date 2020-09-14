using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityUtility;

namespace UnityUtilityTools
{
    public enum MemberCloneOption
    {
        Clone,
        Copy,
        Ignore
    }

    public enum CloneOption
    {
        Full,
        AsICloneable,
        FieldsAsICloneable,
        AsICloneableOrFieldsAsICloneable
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CloneableAttribute : Attribute
    {
        public readonly MemberCloneOption Option;

        public CloneableAttribute(MemberCloneOption option)
        {
            Option = option;
        }
    }

    public static class Helper
    {
        public static int GetHashCode(int hc0, int hc1)
        {
            return hc0 ^ hc1 << 2;
        }

        public static int GetHashCode(int hc0, int hc1, int hc2)
        {
            return hc0 ^ hc1 << 2 ^ hc2 >> 2;
        }

        public static int GetHashCode(int hc0, int hc1, int hc2, int hc3)
        {
            return hc0 ^ hc1 << 2 ^ hc2 >> 2 ^ hc3 >> 1;
        }

        /// <summary>
        /// Clones any object with public or private default constructor.
        /// </summary>
        public static object CloneObject(object sourceObj, CloneOption option = CloneOption.Full)
        {
            if (sourceObj is null)
                return null;

            if (option == CloneOption.AsICloneable && sourceObj is ICloneable cloneable)
                return cloneable.Clone();

            if (sourceObj is Pointer)
                return sourceObj;

            var type = sourceObj.GetType();

            if (type.IsArray)
            {
                var sourceArray = sourceObj as Array;
                var destArray = Array.CreateInstance(type.GetElementType(), sourceArray.Length);

                for (var i = 0; i < sourceArray.Length; i++)
                {
                    var sourceValue = sourceArray.GetValue(i);
                    if (sourceValue == null) { continue; }

                    destArray.SetValue(CloneObject(sourceValue, f_convertToFieldOption(option)), i);
                }

                return destArray;
            }

            var destObj = Activator.CreateInstance(type, true);

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (var i = 0; i < fields.Length; i++)
            {
                var sourceFieldValue = fields[i].GetValue(sourceObj);

                if (sourceFieldValue == null)
                    continue;

                var attribute = Attribute.GetCustomAttribute(fields[i], typeof(CloneableAttribute)) as CloneableAttribute;
                var cloneOption = attribute == null ? MemberCloneOption.Clone : attribute.Option;

                if (cloneOption == MemberCloneOption.Ignore)
                    continue;

                if (cloneOption == MemberCloneOption.Copy)
                {
                    fields[i].SetValue(destObj, sourceFieldValue);
                    continue;
                }

                var AsICloneable = option == CloneOption.FieldsAsICloneable || option == CloneOption.AsICloneableOrFieldsAsICloneable;

                if (AsICloneable && sourceFieldValue is ICloneable sourceFieldValueCloneable)
                {
                    fields[i].SetValue(destObj, sourceFieldValueCloneable.Clone());
                    continue;
                }

                fields[i].SetValue(destObj, CloneObject(sourceFieldValue, f_convertToFieldOption(option)));
            }

            return destObj;
        }

        /// <summary>
        /// Clones any object with public or private default constructor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Clone<T>(T sourceObj, CloneOption option = CloneOption.Full)
        {
            return (T)CloneObject(sourceObj, option);
        }

        // -- //

        private static CloneOption f_convertToFieldOption(CloneOption option)
        {
            switch (option)
            {
                case CloneOption.Full:
                case CloneOption.AsICloneable:
                    return CloneOption.Full;

                default:
                    return CloneOption.AsICloneableOrFieldsAsICloneable;
            }
        }

        internal static string CutAssemblyQualifiedName(string assemblyQualifiedName)
        {
            const char DEVIDER = ',';

            var first = false;

            for (var i = 0; i < assemblyQualifiedName.Length; i++)
            {
                if (assemblyQualifiedName[i] != DEVIDER) { continue; }
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

        public static int Compare<T>(T a, T b) where T : class, IComparable<T>
        {
            if (a != null)
                return a.CompareTo(b);
            if (b != null)
                return -b.CompareTo(a);
            return 0;
        }
    }
}