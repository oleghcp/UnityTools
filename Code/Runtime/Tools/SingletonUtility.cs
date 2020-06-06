using System;
using System.Reflection;
using UnityUtility.Scripts;

namespace Tools
{
    internal static class SingletonUtility
    {
        public static T CreateInstance<T>(Func<T> alteredCreater) where T : class
        {
            var attribute = typeof(T).GetCustomAttribute<CreateInstanceAttribute>(true);
            return attribute != null ? attribute.Create() as T : alteredCreater();
        }
    }
}
