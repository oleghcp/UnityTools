using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnityUtility
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

            Type type = sourceObj.GetType();

            if (type.IsArray)
            {
                Array sourceArray = sourceObj as Array;
                Array destArray = Array.CreateInstance(type.GetElementType(), sourceArray.Length);

                for (int i = 0; i < sourceArray.Length; i++)
                {
                    var sourceValue = sourceArray.GetValue(i);
                    if (sourceValue == null) { continue; }

                    destArray.SetValue(CloneObject(sourceValue, f_convertToFieldOption(option)), i);
                }

                return destArray;
            }

            object destObj = Activator.CreateInstance(type, true);

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (var i = 0; i < fields.Length; i++)
            {
                object sourceFieldValue = fields[i].GetValue(sourceObj);

                if (sourceFieldValue == null)
                    continue;

                CloneableAttribute attribute = Attribute.GetCustomAttribute(fields[i], typeof(CloneableAttribute)) as CloneableAttribute;
                MemberCloneOption cloneOption = attribute == null ? MemberCloneOption.Clone : attribute.Option;

                if (cloneOption == MemberCloneOption.Ignore)
                    continue;

                if (cloneOption == MemberCloneOption.Copy)
                {
                    fields[i].SetValue(destObj, sourceFieldValue);
                    continue;
                }

                bool AsICloneable = option == CloneOption.FieldsAsICloneable || option == CloneOption.AsICloneableOrFieldsAsICloneable;

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

            bool first = false;

            for (int i = 0; i < assemblyQualifiedName.Length; i++)
            {
                if (assemblyQualifiedName[i] != DEVIDER) { continue; }
                if (!first) { first = true; }
                else { return assemblyQualifiedName.Substring(0, i); }
            }

            return null;
        }
    }
}
