using System;
using UnityUtility.Tools;

namespace UnityUtility.CSharp
{
    public static class CommonExtensions
    {
        public static WeakReference<T> ToWeak<T>(this T self) where T : class
        {
            return new WeakReference<T>(self);
        }

        public static WeakReference<T> ToWeak<T>(this T self, bool trackResurrection) where T : class
        {
            return new WeakReference<T>(self, trackResurrection);
        }

        public static T GetTarget<T>(this WeakReference<T> self) where T : class
        {
            self.TryGetTarget(out var target);
            return target;
        }

        public static string ToString(this int self, int radix)
        {
            return ConvertUtility.DecimalToStringWithCustomRadix(self, radix);
        }

        public static string ToString(this long self, int radix)
        {
            return ConvertUtility.DecimalToStringWithCustomRadix(self, radix);
        }

        /// <summary>
        /// Retrieves the name of the constant in the specified enumeration that has the specified value.
        /// </summary>        
        public static string GetName(this Enum self)
        {
            return Enum.GetName(self.GetType(), self);
        }

        /// <summary>
        /// Returns the underlying type code of the specified Type.
        /// </summary>
        public static TypeCode GetTypeCode(this Type self)
        {
            return Type.GetTypeCode(self);
        }

        /// <summary>
        /// Returns simplified assembly qualified name.
        /// </summary>
        public static string GetTypeName(this Type self)
        {
            return Helper.SimplifyTypeName(self.AssemblyQualifiedName);
        }

        /// <summary>
        /// Returns default value for the specified Type.
        /// </summary>
        public static object GetDefaultValue(this Type self)
        {
            return Helper.GetDefaultValue(self);
        }

        public static bool IsAssignableTo(this Type self, Type targetType)
        {
            if (targetType == null)
                return false;

            return targetType.IsAssignableFrom(self);
        }
    }
}
