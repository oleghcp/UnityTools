using System;
using System.Reflection;
using UnityUtility.Scripts;

namespace UnityUtility
{
    internal static class SingletonUtility
    {
        public static T CreateInstance<T>(Func<T> alteredCreater) where T : class
        {
            CreateInstanceAttribute attribute = typeof(T).GetCustomAttribute<CreateInstanceAttribute>(true);
            return attribute != null ? attribute.Create() as T : alteredCreater();
        }
    }
}
