using System.Collections.Generic;
using UnityEngine.Events;
using UnityObject = UnityEngine.Object;

namespace OlegHcp.Engine.Events
{
    public static class UnityEventExtensions
    {
        public static IEnumerable<UnityObject> EnumerateListeners(this UnityEventBase self)
        {
            int count = self.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                yield return self.GetPersistentTarget(i);
            }
        }
    }
}
