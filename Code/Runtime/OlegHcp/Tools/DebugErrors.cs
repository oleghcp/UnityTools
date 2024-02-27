using UnityEngine;

namespace OlegHcp.Tools
{
    internal class DebugErrors
    {
        public static void MultipleInstancesMessage<T>()
        {
            Debug.LogError($"More than one instance of {typeof(T).Name}.");
        }
    }
}
