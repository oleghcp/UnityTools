using System.Reflection;

namespace UnityUtility.SingleScripts
{
    internal static class SingletonUtility
    {
        public static bool TryUseAttribute<T>(out T value) where T : class
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
