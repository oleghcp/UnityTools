#if UNITY_2019_3_OR_NEWER
using System;

namespace UnityUtility.AiSimulation
{
    [Serializable]
    public abstract class StateFinalizer
    {
        public abstract void OnComlete(AiBehaviorSet owner);
    }
}
#endif
