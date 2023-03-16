using System;
using System.Reflection;

namespace UnityUtility.SingleScripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class CreateInstanceAttribute : Attribute
    {
        public abstract void Create();

        internal static bool TryUse<T>() where T : class
        {
            CreateInstanceAttribute attribute = typeof(T).GetCustomAttribute<CreateInstanceAttribute>(true);

            if (attribute == null)
                return false;

            attribute.Create();
            return true;
        }
    }
}
