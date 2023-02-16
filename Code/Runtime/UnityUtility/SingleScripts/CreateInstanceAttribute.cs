using System;
using System.Reflection;

namespace UnityUtility.SingleScripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class CreateInstanceAttribute : Attribute
    {
        public abstract object Create();

        internal static bool TryUse<T>(out T value) where T : class
        {
            CreateInstanceAttribute attribute = typeof(T).GetCustomAttribute<CreateInstanceAttribute>(true);

            if (attribute == null)
            {
                value = default;
                return false;
            }

            value = (T)attribute.Create();
            return true;
        }
    }
}
