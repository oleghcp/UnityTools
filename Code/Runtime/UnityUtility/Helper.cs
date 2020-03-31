using System;
using System.Reflection;

namespace UnityUtility
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NonClonedAttribute : Attribute { }

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
        public static object Clone(object sourceObj, bool considerICloneables = false)
        {
            if (sourceObj is null)
                return null;

            if (considerICloneables && sourceObj is ICloneable)
                return (sourceObj as ICloneable).Clone();

            if (sourceObj is Pointer)
                return sourceObj;

            Type type = sourceObj.GetType();

            if (type.GetTypeCode() != TypeCode.Object)
                return sourceObj;

            if (sourceObj is Delegate)
                return (sourceObj as Delegate).Clone();

            if (type.IsArray)
            {
                Array sourceArray = sourceObj as Array;
                Type elementType = type.GetElementType();

                if (elementType.GetTypeCode() != TypeCode.Object)
                    return sourceArray.Clone();

                Array destArray = Array.CreateInstance(elementType, sourceArray.Length);

                for (int i = 0; i < sourceArray.Length; i++)
                {
                    object sourceValue = sourceArray.GetValue(i);
                    if (sourceValue == null) { continue; }
                    destArray.SetValue(Clone(sourceValue, considerICloneables), i);
                }

                return destArray;
            }

            object destObj = Activator.CreateInstance(type, true);

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < fields.Length; i++)
            {
                if (Attribute.IsDefined(fields[i], typeof(NonClonedAttribute))) { continue; }
                object sourceFieldValue = fields[i].GetValue(sourceObj);
                if (sourceFieldValue == null) { continue; }
                fields[i].SetValue(destObj, Clone(sourceFieldValue, considerICloneables));
            }

            return destObj;
        }

        // -- //

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
